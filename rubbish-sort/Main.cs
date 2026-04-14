using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata;

public partial class Main : Node2D
{
    public static Main Instance;
    public static Dictionary gameData;

    //game variables
    public int test
    {
        get => (int)gameData["test"];
        set
        {
            if (value > 9999) value = 0;
            gameData["test"] = value;
            experimentLabel.Text = $"experiment #3-{robot:00}-{test:0000}";
        }
    }
    public int robot
    {
        get => (int)gameData["robot"];
        set
        {
            if (value > 99) value = 0;
            gameData["robot"] = value;
            experimentLabel.Text = $"experiment #3-{robot:00}-{test:0000}";
        }
    }

    //game visual variables

    private Label experimentLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //initialize visual variables

        experimentLabel = GetNode<Label>("OverScreen/ExperimentLabel");

        //initialize game data

        gameData = new Dictionary();
        gameData["_mute"] = false;
        gameData["test"] = 1;
        gameData["robot"] = 1;
        Instance = this;

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
            experimentLabel.Text = $"experiment #3-{robot:00}-{test:0000}";
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

public enum ItemType
{
    plastic, paper, metal, glass, cardboard
}
public partial class Item : CharacterBody2D
{
    Item selected;
    ItemType type;
    public Item(ItemType type)
    {
        this.type = type;

        CollisionMask = 1;
        CollisionLayer = 3;
    }
    public override void _Ready()
    {

    }
    const int Speed = 200;
    const int Acceleration = 500;
    public override void _PhysicsProcess(double delta)
    {
        if (selected == this)
        {
            var mousePos = GetGlobalMousePosition();
            var direction = (mousePos - GlobalPosition);
            Vector2 velocity = Velocity;
            // Add the gravity.

            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            if (direction != Vector2.Zero)
            {
                velocity.X = Mathf.MoveToward(velocity.X, Speed * direction.X, Acceleration * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, Speed * direction.Y, Acceleration * (float)delta);
            }
            else
            {
                velocity.X = Mathf.MoveToward(velocity.X, 0, Acceleration * (float)delta);
                velocity.Y = Mathf.MoveToward(velocity.Y, 0, Acceleration * (float)delta);
            }
            velocity += GetGravity();


            Velocity = velocity;
        }
        MoveAndSlide();
    }
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("lclick"))
        {
            var mousePos = GetGlobalMousePosition();
            var spaceState = GetWorld2D().DirectSpaceState;
            var param = new PhysicsPointQueryParameters2D();
            param.Position = mousePos;
            param.CollisionMask = 2;
            var result = spaceState.IntersectPoint(param);
            if (result.Count > 0)
            {
                var collider = (Node)result[0]["collider"];
                if (collider is Item item)
                {
                    selected = item;
                }
            }
        }
        else if (Input.IsActionJustReleased("lclick"))
        {
            selected = null;
        }
    }

}