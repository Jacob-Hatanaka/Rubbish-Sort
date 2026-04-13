using Godot;
using Godot.Collections;
using System;

public partial class Main : Node2D
{
    public static Main Instance;
    public static Dictionary gameData;
    public int test
    {
        get => (int)gameData["test"];
        set
        {
            gameData["test"] = value;
        }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameData["_mute"] = false;
        gameData["test"] = 0;
        Instance = this;
        gameData = new Dictionary();
        //load data

        //disable auto accept quit to allow for saving before quitting
        GetTree().AutoAcceptQuit = false;

        // load locally saved data
        var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);

        //make sure file exists
        if (file != null && !file.EofReached())
        {
            //get dictionary of data
            Dictionary saveData = (Dictionary)file.GetVar();

            // close cuz unneeded and prevents issues
            file.Close();
            foreach (var kvp in saveData)
            {
                gameData[kvp.Key] = kvp.Value;
            }
            //money = money;
            //plotlevel = plotlevel;
            //plantcount = plantcount;
        }
        //test below
        //money = 100000;
    }
    void _notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            // pre save closure

            //add points on current run
            //addTotalPts(ptsOnCurrentRun);

            // do save stuff here
            Dictionary saveData = new Dictionary();
            saveData = gameData;
            /*saveData["totalPts"] = totalPts;
			saveData["skillsUnlocked"] = skillsUnlocked;
			saveData["skillPts"] = skillPts;
			saveData["level"] = level;*/
            // save to file
            var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
            file.StoreVar(saveData);
            file.Close();
            GD.Print("Saving game...");
            GetTree().Quit();
        }
    }
    void resetAll()
    {
        DirAccess.RemoveAbsolute("user://savegame.save");
        GetTree().Quit();
    }
    void exitButtonPressed()
    {
        //uncomment to get the prep direction code for skills printed to console
        //SkillBubble.prepDirectionCode();
        GetTree().Quit();
    }
    public enum SoundType
    {
        sfx, music, menu
    }
    bool _mute
    {
        get => (bool)gameData["_mute"];
        set
        {
            gameData["_mute"] = value;
        }
    }
    public static void sound(string sound, SoundType soundtype = SoundType.sfx)
    {
        if (Instance._mute) return;
        if (FileAccess.FileExists("res://assets/audio/" + sound + ".wav") == false)
        {
            GD.Print("sound: " + sound + " of type " + soundtype.ToString());
            return;
        }
        var soundwav = GD.Load<AudioStreamWav>("res://assets/audio/" + sound + ".wav");
        if (soundwav == null)
        {
            GD.Print("sound: " + sound + " of type " + soundtype.ToString());
            return;
        }

        return;
    }
    public void mute()
    {
        _mute = true;
    }
    public void unmute()
    {
        _mute = false;
    }
    public void toggleMute()
    {
        _mute = !_mute;
    }
}
