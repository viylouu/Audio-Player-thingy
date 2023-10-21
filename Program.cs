using NAudio.Wave;
using ImGuiNET;
using System.Numerics;
using SimulationFramework;
using SimulationFramework.Drawing;

int songDirs = 0;

int songSel = -1;

double songDur = 0;
float songPos = 0;

WaveStream[] ins = new WaveStream[0];
WaveOutEvent[] outs = new WaveOutEvent[0];

Random r = new Random();

bool restartSong = false;

Simulation sim = Simulation.Create(Init, Rend);
sim.Run();

void Init()
{
    Window.Title = "Audio Player";

    songDirs = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Songs\", "*.wav").Length;

    ins = new WaveStream[songDirs];
    outs = new WaveOutEvent[songDirs];

    for (int i = 0; i < songDirs; i++)
    { ins[i] = new WaveFileReader(Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Songs\", "*.wav")[i]); outs[i] = new WaveOutEvent(); }
}

void Rend(ICanvas canv)
{
    canv.Clear(SimulationFramework.Color.Black);

    if (ins.Length != 0)
    {
        if (songSel == -1 || (songSel != -1 ? (outs[songSel].PlaybackState == PlaybackState.Stopped ? true : false) : true) || restartSong)
        {
            if (restartSong)
            {
                restartSong = false;

                for (int i = 0; i < outs.Length; i++)
                {
                    if (outs[i].PlaybackState is PlaybackState.Playing) { outs[i].Stop(); }
                }
            }

            songSel = r.Next(songDirs);

            if (outs[songSel].PlaybackState is PlaybackState.Playing) { outs[songSel].Stop(); }
            ins[songSel].CurrentTime = new TimeSpan(0L);
            outs[songSel].Init(ins[songSel]);
            outs[songSel].Play();

            songDur = ins[songSel].TotalTime.TotalSeconds;
        }

        songPos = (float)ins[songSel].CurrentTime.TotalSeconds;
    }

    ImGuiStylePtr style = ImGui.GetStyle();
    style.WindowBorderSize = 0;
    style.WindowRounding = 10;
    style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(15f / 255f, 10f / 255f, 21f / 255f, 1);
    style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(22f / 255f, 15f / 255f, 31f / 255f, 1);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(15f / 255f, 10f / 255f, 21f / 255f, 1);
    style.Colors[(int)ImGuiCol.Button] = new Vector4(194f / 255f, 109f / 255f, 36f / 255f, 1);
    style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(214f / 255f, 129f / 255f, 56f / 255f, 1);
    style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(163f / 255f, 89f / 255f, 24f / 255f, 1);
    style.Colors[(int)ImGuiCol.Text] = new Vector4(216f / 255f, 248f / 255f, 247f / 255f, 1);

    ImGui.Begin("AudioPlayer");

    ImGui.SetWindowSize(new Vector2(Window.Width, Window.Height));
    ImGui.SetWindowPos(new Vector2(Window.Width / 2 - ImGui.GetWindowSize().X / 2, Window.Height / 2 - ImGui.GetWindowSize().Y / 2));

    if (ImGui.Button("Skip Song"))
    { restartSong = true; }

    if (ImGui.Button("Close"))
    { Environment.Exit(0); }

    ImGui.Text(songDur + "");
    ImGui.Text(songPos + "");

    for (int i = 0; i < 26; i++)
    {
        ImGui.Text("");
    }

    ImGui.SliderFloat("Song Progress", ref songPos, 0, (float)songDur);
    ImGui.Text("Song Playing: " + Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Songs\", "*.wav")[songSel].Remove(0, (Directory.GetCurrentDirectory() + @"\Songs\").Length));

    ImGui.End();
}