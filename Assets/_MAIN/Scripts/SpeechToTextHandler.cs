using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class SpeechToTextHandler : MonoBehaviour
{
    [SerializeField] private string Url;
    [SerializeField] private string Key;
    public UnityEvent OnText = new UnityEvent();

    public void Convert(AudioClip clip)
    {
        byte[] wav = AudioClip_to_Wav(clip);
        System.IO.File.WriteAllBytes(Application.dataPath + "/out.wav", wav);
    }

    private byte[] AudioClip_to_Wav(AudioClip clip)
    {
        // Build body
        float[] samples = new float[clip.samples];
        Byte[] body = new byte[samples.Length * 2];

        int rescaleFactor = 32767; // convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            byte[] sampleBytes = BitConverter.GetBytes((Int16)(samples[i] * rescaleFactor));
            Buffer.BlockCopy(sampleBytes, 0, body, i * 2, 2);
        }


        // Build header
        byte[] header = new byte[46];
        int writeLocation = 0;

        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        Buffer.BlockCopy(riff, 0, header, 0, writeLocation);
        writeLocation += 4;

        byte[] chunkSize = BitConverter.GetBytes((clip.samples * 2) - 8);
        Buffer.BlockCopy(chunkSize, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        Buffer.BlockCopy(wave, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        Buffer.BlockCopy(fmt, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] subChunk1 = BitConverter.GetBytes(16);
        Buffer.BlockCopy(subChunk1, 0, header, writeLocation, 4);
        writeLocation += 4;

        UInt16 two = 2;
        UInt16 one = 1;

        byte[] audioFormat = BitConverter.GetBytes(one);
        Buffer.BlockCopy(audioFormat, 0, header, writeLocation, 2);
        writeLocation += 2;

        byte[] numChannels = BitConverter.GetBytes(clip.channels);
        Buffer.BlockCopy(numChannels, 0, header, writeLocation, 2);
        writeLocation += 2;

        byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
        Buffer.BlockCopy(sampleRate, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        Buffer.BlockCopy(byteRate, 0, header, writeLocation, 4);
        writeLocation += 4;

        UInt16 blockAlign = (ushort)(clip.channels * 2);
        Buffer.BlockCopy(BitConverter.GetBytes(blockAlign), 0, header, writeLocation, 2);
        writeLocation += 4;

        UInt16 bps = 16;
        byte[] bitsPerSample = BitConverter.GetBytes(bps);
        Buffer.BlockCopy(bitsPerSample, 0, header, writeLocation, 2);
        writeLocation += 2;

        byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        Buffer.BlockCopy(datastring, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
        Buffer.BlockCopy(subChunk2, 0, header, writeLocation, 4);
        writeLocation += 4;

        byte[] wav = new byte[header.Length + body.Length];
        Buffer.BlockCopy(header, 0, wav, 0, header.Length);
        Buffer.BlockCopy(body, 0, wav, header.Length, body.Length);

        return wav;
    }
}
