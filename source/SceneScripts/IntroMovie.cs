using Godot;
using System;

public class IntroMovie : VideoPlayer
{
	private void _on_IntroMovie_finished()
	{
		GetTree().ChangeScene("MainMenu.tscn");
	}
}


