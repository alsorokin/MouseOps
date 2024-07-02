using MouseOps;
using static MouseOps.MouseOperations;

Console.WriteLine("Hello, World!");

MousePoint position = GetMousePosition();
int centerX = position.X;  // Set the center x coordinate
int centerY = position.Y;  // Set the center y coordinate
int radius = 50;    // Set the radius of the circle
int steps = 36;     // Set the number of steps to complete a circle (360 degrees / 36 = 10 degree steps)
int clickPositionRandomness = 5; // Set the randomness of the click position
int clickDelayRandomness = 5;    // Set the randomness of the click delay
int resumeCooldown = 5000;       // Set the cooldown before resuming auto-clicking after mouse movement (in milliseconds)

double angleStep = 2 * Math.PI / steps;
int currentStep = 0;
bool isMouseDown = false;
MousePoint lastMousePosition = position;
ulong clicks = 0;

AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
while (true)
{
    MousePoint currentPosition = GetMousePosition();
    if (currentPosition.X != lastMousePosition.X || currentPosition.Y != lastMousePosition.Y)
    {
        Console.WriteLine($"Mouse moved, sleeping. Clicks so far: {clicks}");
        WaitForMouseStill();
        Console.WriteLine("Mouse is still, resuming.");
    }

    // Calculate new mouse position
    int newXRandomness = new Random().Next(-clickPositionRandomness, clickPositionRandomness);
    int newYRandomness = new Random().Next(-clickPositionRandomness, clickPositionRandomness);
    int newX = centerX + (int)(radius * 2 * Math.Cos(currentStep * angleStep)) + newXRandomness;
    int newY = centerY + (int)(radius * Math.Sin(currentStep * angleStep)) + newYRandomness;

    // Move the mouse to the new position
    MouseOperations.SetCursorPosition(newX, newY);
    lastMousePosition = new MousePoint(newX, newY);

    // Click the mouse
    MouseOperations.MouseEvent(MouseEventFlags.LeftDown);
    isMouseDown = true;
    Thread.Sleep(5);
    MouseOperations.MouseEvent(MouseEventFlags.LeftUp);
    clicks++;
    isMouseDown = false;

    // Increment step
    currentStep = (currentStep + 1) % steps;

    // Sleep before next movement
    int delay = new Random().Next(0, clickDelayRandomness);
    Thread.Sleep(20 + delay);
}

void OnProcessExit(object? sender, EventArgs e)
{
    if (isMouseDown)
    {
        MouseOperations.MouseEvent(MouseEventFlags.LeftUp);
        Console.WriteLine($"Clicks: {clicks}");
    }
}

void WaitForMouseStill()
{
    MousePoint lastPos = GetMousePosition();
    int timeNotMoved = 0;
    const int sleepInterval = 100;
    while (true)
    {
        Thread.Sleep(sleepInterval);
        MousePoint currentPos = GetMousePosition();
        if (currentPos.X == lastPos.X && currentPos.Y == lastPos.Y)
        {
            timeNotMoved += sleepInterval;
            if (timeNotMoved >= resumeCooldown)
            {
                break;
            }
        }
        else
        {
            timeNotMoved = 0;
            lastPos = currentPos;
        }
    }
}
