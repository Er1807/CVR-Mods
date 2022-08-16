using ABI_RC.Core.InteractionSystem;
using Dissonance.Audio.Capture;
using HarmonyLib;
using MelonLoader;
using MuteTTS;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

[assembly: MelonInfo(typeof(MuteTTSMod), "MuteTTS", "1.1.1", "Eric van Fandenfart")]
[assembly: MelonGame]

namespace MuteTTS
{
    public class MuteTTSMod : MelonMod
    {
        private static MemoryStream stream = new MemoryStream();
        private static AudioSource audiosource = null;
        private static bool playing = false;
        private static bool lastMuteValue;
        private string lastLineRead;
        private string exeLocation;
        private MelonPreferences_Entry<int> useVoiceSetting;
        private static MelonPreferences_Entry<bool> blockMic;
        private static MelonPreferences_Entry<float> TTSSpeed;
        private static MelonPreferences_Entry<float> TTSVolume;

        //MuteTTS.MuteTTSMod.Instance.GetVoice("test")
        public static MuteTTSMod Instance;

        public static Action<string> OnKeyboardSubmitted;

        public override void OnApplicationStart()
        {
            //MuteTTS.MuteTTSMod.Instance.useVoiceSetting.Value = 1;
            Instance = this;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MelonLogger.Msg("MuteTTS is only available for Windows");
                return;
            }

            MelonPreferences_Category category = MelonPreferences.CreateCategory("MuteTTS");
            useVoiceSetting = category.CreateEntry("UseVoice", -1);

            blockMic = category.CreateEntry("BlockMic", false, description: "VRC will no longer be able to send your Voice. Only TTS is available");
            TTSVolume = category.CreateEntry("TTS Volume", 1f, description: "Value between 0 and 1");
            TTSSpeed = category.CreateEntry("TTS Speed", 1f);

            MelonCoroutines.Start(WaitForMainMenuView());

            ExtractExecutable();

            LogAvailableVoices();

            HarmonyInstance.Patch(typeof(BasicMicrophoneCapture).GetMethod("ConsumeSamples", BindingFlags.Instance | BindingFlags.NonPublic), prefix: new HarmonyMethod(typeof(MuteTTSMod).GetMethod("ConsumeSamples", BindingFlags.Static | BindingFlags.Public)));
        }

        private void ExtractExecutable()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string path = "Executables";
                exeLocation = Path.GetFullPath(path + "/MuteTTSClient.exe");

                Directory.CreateDirectory(path);
                using (var stream = assembly.GetManifestResourceStream("MuteTTS.MuteTTSClient.exe"))
                {
                    using (FileStream file = new FileStream(exeLocation, FileMode.Create))
                        stream.CopyTo(file);
                }
                MelonLogger.Msg($"Extracted file to {exeLocation}");
            }
            catch (Exception)
            {
                MelonLogger.Msg("Couldnt extract exe file. Maybe file is in use and its not an error");
            }
        }

        private void LogAvailableVoices()
        {
            ProcessStartInfo startInfo = CreateDefaultStartInfo();

            startInfo.Arguments = $"ListVoices";
            MelonLogger.Msg("Available Voices:");
            using (Process exeProcess = Process.Start(startInfo))
            {
                _ = ConsumeReader(exeProcess.StandardOutput, true);
                exeProcess.WaitForExit();
            }
        }

        private async Task ConsumeReader(TextReader reader, bool justPrint)
        {
            string text;

            while ((text = await reader.ReadLineAsync()) != null)
            {
                if (justPrint)
                    MelonLogger.Msg(text);
                else
                    lastLineRead = text;
            }
        }

        public void GetVoice(string msg = "Hello World")
        {
            Task.Run(() => {
                ProcessStartInfo startInfo = CreateDefaultStartInfo();

                msg = msg.Replace("\\", "").Replace("\"", "");

                startInfo.Arguments = $"PlayVoice \"{msg}\" {useVoiceSetting.Value} {Convert.ToString(TTSSpeed.Value, CultureInfo.InvariantCulture)} {Convert.ToString(TTSVolume.Value, CultureInfo.InvariantCulture)}";

                MelonLogger.Msg($"Calling excutable with parameters {startInfo.Arguments.Replace(msg, "***")}");

                using (Process exeProcess = Process.Start(startInfo))
                {
                    Task reader = ConsumeReader(exeProcess.StandardOutput, false);
                    exeProcess.WaitForExit();
                    reader.Wait();
                    byte[] buffer = Convert.FromBase64String(lastLineRead);
                    MelonLogger.Msg($"Recieved {buffer.Length} bytes from excutable to play");
                    stream = new MemoryStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Position = 0;

                    if (audiosource == null)
                        audiosource = CreateAudioSource();
                    audiosource.clip = CreateAudioClipFromStream(buffer);
                    //audiosource.outputAudioMixerGroup = GetVoiceAudioMixerGroup();
                    audiosource.Play();
                    playing = true;
                    //lastMuteValue = micToggle.isOn;
                    //micToggle.onValueChanged.Invoke(true);
                }
            });
        }

        private AudioClip CreateAudioClipFromStream(byte[] buffer)
        {
            AudioClip myClip = AudioClip.Create("MuteTTS", buffer.Length, 1, 48000, false);
            float[] t = new float[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                t[i] = ((float)buffer[i] - 128) / 128;

            }
            myClip.SetData(t, 0);
            return myClip;
        }

        private ProcessStartInfo CreateDefaultStartInfo()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = exeLocation,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            return startInfo;
        }

        public AudioSource CreateAudioSource()
        {
            GameObject obj = new GameObject("MuteTTS");
            GameObject.DontDestroyOnLoad(obj);

            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0;

            return audioSource;
        }


        public static void ConsumeSamples(ref ArraySegment<float> samples)
        {
            if (playing)
            {
                byte[] buffer = new byte[samples.Count];
                int read = stream.Read(buffer, 0, samples.Count);

                if (read == 0)
                {
                    playing = false;
                    //if (micToggle != null)
                    //    micToggle.onValueChanged.Invoke(lastMuteValue);
                    return;
                }

                for (int i = 0; i < samples.Count; i++)
                {
                    if (i < read)
                        samples.Array[i] = ((float)buffer[i] - 128) / 128;
                    else
                        samples.Array[i] = 0;
                }

            }
            else if (blockMic.Value)
            {
                for (int i = 0; i < samples.Count; i++)
                    samples.Array[i] = 0;
            }

        }

        public void OpenTTSInput()
        {
            OpenKeyboard("", (str) => GetVoice(str));
        }

        public static void OpenKeyboard(string currentValue, Action<string> callback)
        {
            OnKeyboardSubmitted = callback;
            ViewManager.Instance.openMenuKeyboard(currentValue);
        }

        public IEnumerator WaitForMainMenuView()
        {
            while (ViewManager.Instance == null)
                yield return null;
            while (ViewManager.Instance.gameMenuView == null)
                yield return null;
            while (ViewManager.Instance.gameMenuView.Listener == null)
                yield return null;

            ViewManager.Instance.gameMenuView.Listener.ReadyForBindings += () =>
            {
                ViewManager.Instance.gameMenuView.View.RegisterForEvent("MelonMod_MuteTTS_Action", new Action(OpenTTSInput));
            };

            ViewManager.Instance.gameMenuView.Listener.FinishLoad += (_) =>
            {
                ViewManager.Instance.gameMenuView.View.ExecuteScript(@"﻿{
    var l_block = document.createElement('div');
            l_block.innerHTML = `<h2> MuteTTS </h2><div class=""action-btn"" onclick=""engine.trigger('MelonMod_MuteTTS_Action');"">MuteTTS</div>`;
        document.getElementById('settings-implementation').appendChild(l_block);
    }");
            };
        }
    }

    [HarmonyPatch(typeof(ViewManager))]
    class ViewManagerPatches
    {
        [HarmonyPatch("SendToWorldUi")]
        [HarmonyPostfix]
        static void SendToWorldUi(string value)
        {
            MuteTTSMod.OnKeyboardSubmitted?.Invoke(value);

            MuteTTSMod.OnKeyboardSubmitted = null;
        }
    }
}
