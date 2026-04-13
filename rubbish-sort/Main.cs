using Godot;
using Godot.Collections;
using System;

public partial class Main : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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

            // dont cause memory leak
            file.Close();

            // get total points player has
            //level = (int)saveData["level"];
            //var pts = (int)saveData["totalPts"];
            //setTotalPts(pts);
            //skillPts = (int)saveData["skillPts"];
            //if (skillPts > 0) hasSkillPts = true;

            //var skills = (Dictionary<Skills, int>)saveData["skillsUnlocked"];
            //foreach (Skills skill in Enum.GetValues(typeof(Skills)))
            //{
            //    if (skills.ContainsKey(skill))
            //    {
            //        skillsUnlocked[skill] = skills[skill];
            //    }
            //}
        }
        //generate enemy on base floor to test
        //instantiateEnemy(enemyList[3]); 
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
            //saveData["totalPts"] = totalPts;
            //saveData["skillsUnlocked"] = skillsUnlocked;
            //saveData["skillPts"] = skillPts;
            //saveData["level"] = level;
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
    public void sound(string sound, SoundType soundtype = SoundType.sfx)
    {
        var soundwav = GD.Load<AudioStreamWav>("res://assets/audio/" + sound + ".wav");
        if (soundwav == null)
        {
            //GD.Print("sound: " + sound + " of type " + soundtype.toString());
            return;
        }

        return;
    }
}
