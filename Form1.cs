using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.IO;

namespace ProjectLuna
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine LunaBrain = new SpeechRecognitionEngine();
        SpeechSynthesizer LunaVoice = new SpeechSynthesizer();
        bool trigger = false ;
        string User = Environment.UserDomainName;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnVoice_Click(object sender, EventArgs e)
        {
            if (trigger == false)
            {
                LunaBrain.RecognizeAsync(RecognizeMode.Multiple);
                trigger = true;
                btnVoice.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.micActive));
            }
            else {
                LunaBrain.RecognizeAsyncStop();
                btnVoice.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.micOff));
                trigger = false;
            }
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Basic Commands for Luna 
            //Access Luna
            Choices BasicuserCommands = new Choices();
            BasicuserCommands.Add(new string[] {"Hello Luna","Goodbye Luna"});
            GrammarBuilder basicActionGrammar = new GrammarBuilder();
            basicActionGrammar.Append(BasicuserCommands);
            Grammar basicGrammar = new Grammar(basicActionGrammar);
            LunaBrain.LoadGrammarAsync(basicGrammar);

            //Disable mircophone
            Choices microphone = new Choices();
            microphone.Add(new string[] { "Disable Mic" });
            GrammarBuilder microphoneGrammarBuilder = new GrammarBuilder();
            microphoneGrammarBuilder.Append(microphone);
            Grammar microphoneGrammar = new Grammar(microphoneGrammarBuilder);
            LunaBrain.LoadGrammarAsync(microphoneGrammar);

            //Commands that involve the pc
            Choices pcCommands = new Choices();
            pcCommands.Add(new string[] { "Open Notepad" });
            GrammarBuilder pcGrammarBuilder = new GrammarBuilder();
            pcGrammarBuilder.Append(pcCommands);
            Grammar pcGrammar = new Grammar(pcGrammarBuilder);
            LunaBrain.LoadGrammarAsync(pcGrammar);




            LunaBrain.SetInputToDefaultAudioDevice();
            LunaBrain.SpeechRecognized += LunaBrain_SpeechRecognized;
            LunaVoice.Rate = 0;
            LunaVoice.SelectVoiceByHints(VoiceGender.Female);

            

        }

        private void LunaBrain_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string greeting = "";
            string time = "";
            int hour = 0;
            DateTime date1 = DateTime.Now ;           
            time = date1.ToString("tt");
            hour = int.Parse( date1.ToString("%h"));



            switch (e.Result.Text)
            {
                case "Hello Luna":
                    
                    PromptBuilder builder = new PromptBuilder();
                    if (time == "PM" && hour >=12 && hour < 18) {
                        greeting = "Good Afternoon";
                    }else
                    {
                        greeting = "Good Evening";
                    }
                     if (time == "AM")
                    {
                        greeting = "Good Morning";
                    };

                    builder.AppendText(greeting + " " + User);

                    
                    LunaVoice.Speak(builder);
                    break;

                case "Goodbye Luna":
                    PromptBuilder Goodbye = new PromptBuilder();
                    Goodbye.AppendText("Goodbye " + User);
                    LunaVoice.Speak(Goodbye);
                    Application.Exit();

                    break;

                case "Open Notepad":
                    System.Diagnostics.Process.Start(@"C:\Windows\System32\Notepad.exe");
                    PromptBuilder open = new PromptBuilder();
                    open.AppendText("Opening Notepad ");
                    LunaVoice.Speak(open);

                    break;

                case "Disable Mic":
                    LunaBrain.RecognizeAsyncStop();
                    PromptBuilder off = new PromptBuilder();
                    off.AppendText("Your microphone is now disabled!");
                    LunaVoice.Speak(off);
                    btnVoice.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.micOff));
                    trigger = false;
                    break;
                    
            }
        }
    }
}
