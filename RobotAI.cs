﻿using System;
using TriCAT;

public class SampleRobot : IRobotPlugin
{
	// Name der Lerngruppe, die dieses Plugin entwickelt
	public string GroupName { get { return "Test Group"; } }

	// Liste mit Email Adressen der Authoren
	public string[] Authors { get { return new string[] { "max@mustermann.de", "john@doe.com" }; } }

	// Versionsnummer des Plugins
	public int VersionMajor { get { return 1; } }
	public int VersionMinor { get { return 0; } }

    enum State {SearchStone, Ziel, Ende, Pre};
    State RobotState;

    public StoneData [] OurStones;
    public int counter = 0;

    
    

	// Diese Interfaces bekommt man beim Start übergeben und sollte sie sich zur weiteren verwendung speichern.
	private IApplication App;
	private IRobot Robot;
	private IPlayingField Field;

	// Zum Speichern des aktuellen Steins
	private StoneData currentStone = new StoneData();
    private Vector currentStonePos;

	/// <summary>
	/// Called once, after the plugin has been loaded
	/// </summary>
	/// <param name="app">Provides some debug methods like message logging and line drawing</param>
	/// <param name="robot">Holds the state of the actual robot and exposes some driving controls</param>
	/// <param name="field">Holds the state of the playing field, like active stones and target area</param>
	public void OnInit(IApplication app, IRobot robot, IPlayingField field)
	{
		// Über diese interfaces kann später (OnTick) auf Daten wie die Position der Steine auf dem Feld usw. zugegriffen werden
		App = app;
		Robot = robot;
		Field = field;

        RobotState = State.Pre;

        

		App.LogMessage("Sample Robot initialized");

        //RobotPos = Robot.Data.Position;
        //StonePos = Field.OperationArea.Position;
        //TargetCircle = Field.TargetArea.Position;

	}

	/// <summary>
	/// Called everytime, the user clicks the "Reset" button.
	/// </summary>
	public void OnReset()
	{
		App.LogMessage("Sample Robot resetted");
	}

	/// <summary>
	/// Called once for every simulation step.
	/// This is the place to implement your robot steering logic.
	/// </summary>
	public void OnTick()
	{
		// In dieser Funktion kann die KI des Roboters implementiert werden, z.B. als State Machine.
		// Die KI muss jeden Tick die Entscheidung treffen, was der Roboter tun soll, zur Wahl stehen: links / rechts drehen, vorwärts / rückwärts fahren, gar nichts tun
		// z.B. im Kreis zu fahren:

		// update local copy of the stone
       // if (currentStone.Id != -1)
        //{
          //  currentStone = Field.GetStoneById(currentStone.Id);
        //}


        if (RobotState == State.Pre)
        {
            

            FindStone();
            RobotState = State.SearchStone;

        }

        
        
        switch (RobotState)
        {
            case State.SearchStone: MoveToStone(); break;
            case State.Ziel: MoveToTarget(); break;
            case State.Pre: App.LogMessage("Pre fail") ; break;
            
        }
        

	}

    public void MoveToStone()
    {
        
            
        
       // Vector x = currentStone.Position - Robot.Data.Position;
        Vector x = currentStonePos - Robot.Data.Position;
        x.Normalize();
        float result = Vector.Dot(Robot.Data.Right, x);

       /* if (currentStone.IsInTouchWithRobot == true)
        {
            // State Ziel ändern
            RobotState = State.Ziel;
        } */
        if(Vector.Dot(Robot.Data.Forward, x) < 0)
        {
            RobotState = State.Ziel;
        }
        else
        {
            Movement(result);
        }


    }

 

    public void MoveToTarget()
    {

       /* if (currentStone.IsInTouchWithRobot == false)
        {
            
            RobotState = State.SearchStone;
            return;
        } */



        Vector x = Field.TargetArea.Position - Robot.Data.Position;
        x.Normalize();
        float result = Vector.Dot(Robot.Data.Right, x);

        


       /* if (currentStone.Id == -1)
        {
            //State SteinSuchen ändern
            RobotState = State.SearchStone;
            FindStone();
            
        } */
        if (Vector.Dot(Robot.Data.Forward, x) < 0)
        {
            Robot.GoBackward();
            Robot.GoBackward();
            Robot.GoBackward();
            Robot.GoBackward();
            Robot.GoBackward();
            RobotState = State.SearchStone;
            FindStone();
        }
        else
        {
            Movement(result);
        } 
    }


    public void FindStone()
    {
      /* int oldStoneId = currentStone.Id;
        currentStone = Field.Stones[0];
        foreach (StoneData s in Field.Stones)
        {
            if( s.Id == oldStoneId)
            {
                continue;
            }
            if (Vector.DistanceSquared(s.Position, Robot.Data.Position) <
                Vector.DistanceSquared(currentStone.Position, Robot.Data.Position))
            {
                currentStone = s;
            }
        }
        currentStonePos  = currentStone.Position;
        */
        if (counter < 4)
        {
            
            currentStone = Field.GetStoneById(counter);
            currentStonePos = currentStone.Position;
            counter++;
            if (currentStone.Id == -1)
            {
           //     App.LogMessage("no stone");
            }
        }
        
    }

    public void Movement(float result)
    {
        
        
        if (result > 0)
        {
            //rechts drehen
            
            Robot.TurnRight();
            Robot.GoForward();
        }

        if (result < 0)
        {
            //links drehen
            Robot.TurnLeft();
            Robot.GoForward();
          
        }

        if (result == 0)
        {
            Robot.GoForward();
        }
        

    //    if (result <= 0.02 && result >= -0.02)
        {
            
        //   Robot.GoForward();
        }
    }

	/// <summary>
	/// Called once before the plugin is unloaded
	/// </summary>
	public void OnRelease()
	{
		App.LogMessage("Sample Robot released");

		App = null;
		Robot = null;
		Field = null;
	}


}

