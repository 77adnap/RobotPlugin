using System;
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

    enum State {SearchStone, Ziel, Ende, Pre, Back};
    State RobotState;

    public StoneData [] OurStones;
    public int counter = 0;
    public int counterBack = 0;
    public int counterPush = 0;
    
    

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
            case State.Back: Back(); break;
            
        }
        

	}

    public void MoveToStone()
    {
        
            
        
        Vector x = currentStonePos - Robot.Data.Position;
        Vector y = x;
        x.Normalize();
        float result = Vector.Dot(Robot.Data.Right, x);

     
        if(Vector.Dot(Robot.Data.Forward, x) < 0 && y.Length() < 1 )
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

        Vector x = Field.TargetArea.Position - Robot.Data.Position;
        Vector y = x;
        x.Normalize();
        float result = Vector.Dot(Robot.Data.Right, x);


       /* if (currentStone.Id == -1)
        {
            //State SteinSuchen ändern
            RobotState = State.Back;
            
           
        }*/ //Vector.Dot(Robot.Data.Forward, x) < 0

        if ( y.Length() < Field.TargetArea.Radius )
        {
            
            RobotState = State.Back;  
        }
        else
        {
            Movement(result);
        } 
    }


    public void FindStone()
    {

        if (counter < 4)
        {
            App.LogMessage(counter + " :");
            currentStone = Field.GetStoneById(counter);
            currentStonePos = currentStone.Position;
            counter++;

        }
        else
        {
            
            for (int i = 0; i < 4; i++)
            {
                if (Field.GetStoneById(i).Id == -1)
                {
                    counter = i; 
                    App.LogMessage("id " + Field.GetStoneById(i).Id);
                }
            }
            
        }
        
    }

    public void Back()
    {
        if (counterBack < 20)
        {
            Robot.GoBackward();
            counterBack++;
        }
        else
        {
            counterBack = 0;
            RobotState = State.SearchStone;
            FindStone();
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

