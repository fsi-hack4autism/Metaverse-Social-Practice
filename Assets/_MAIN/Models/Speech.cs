using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

class Program
{
    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    static string speechKey = "f4734b50ee64422eb2d3e9bdc2a21fba"; // Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = "eastus"; // Environment.GetEnvironmentVariable("SPEECH_REGION");

    static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Console.WriteLine($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }

    async static Task Main(string[] args)
    {
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // The language of the voice that speaks.
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
        {
            speechSynthesizer.VisemeReceived += (s, e) =>
            {
                Console.WriteLine($"Viseme event received. Audio offset: " + $"{e.AudioOffset / 10000}ms, viseme id: {e.VisemeId}.");
                var animation = e.Animation;
            };


            string text = "";

            while (text != "bye")
            {
                // Get text from the console and synthesize to the default speaker.
                Console.WriteLine("Enter some text that you want to speak >");
                text = Console.ReadLine();

                var speechResult = await speechSynthesizer.SpeakTextAsync(text);
                var visemeResult = await speechSynthesizer.SpeakSsmlAsync(text);
                OutputSpeechSynthesisResult(speechResult, text);
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    async static Task foo()
    {
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // The language of the voice that speaks.
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

        using (var synthesizer = new SpeechSynthesizer(speechConfig))

        {

            // Subscribes to viseme received event

            synthesizer.VisemeReceived += (s, e) =>

            {

                Console.WriteLine($"Viseme event received. Audio offset: " +

                    $"{e.AudioOffset / 10000}ms, viseme id: {e.VisemeId}.");



                // `Animation` is an xml string for SVG or a json string for blend shapes

                var animation = e.Animation;

            };



            // If VisemeID is the only thing you want, you can also use `SpeakTextAsync()`

            var result = await synthesizer.SpeakSsmlAsync("good day");

        }
    }
}
