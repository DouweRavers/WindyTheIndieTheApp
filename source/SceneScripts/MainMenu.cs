using Godot;
using System;

public class MainMenu : CanvasLayer
{
	String toLoadScene;
	Timer timer;
	public override void _Ready(){
		timer = ((Timer) GetNode("Timer"));
	}
	
	private void timeout(){
		GetTree().ChangeScene(toLoadScene);
	}
	
	private void _on_InfiniteMode_button_down()
	{
		button_pressed();
		toLoadScene = "res://InfiniteMode.tscn";
	}
	private void _on_Tutorial_button_down()
	{
		button_pressed();
		toLoadScene = "res://Tutorial.tscn";
	}

	private void button_pressed(){
		((AudioStreamPlayer) GetNode("Click")).Play();
		timer.Stop();
		timer.WaitTime = 0.5f;
		timer.Start();
	}
}
