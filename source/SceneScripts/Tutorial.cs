using Godot;
using System;

public class Tutorial : Control
{
	int iterator = 0;
	private void _next(){
		iterator++;
		((AudioStreamPlayer) GetNode("click")).Play();
		switch(iterator){
			case 0:
				((AnimatedSprite) GetNode("Dia")).Animation = "Intro";
				break;
			case 1:
				((AnimatedSprite) GetNode("Dia")).Animation = "Controls";
				break;
			case 2:
				((AnimatedSprite) GetNode("Dia")).Animation = "Game";
				break;
			case 3:
				((AnimatedSprite) GetNode("Dia")).Animation = "Shop";
				break;
			case 4:
				((AnimatedSprite) GetNode("Dia")).Animation = "Pointers";
				break;
			case 5:
				GetTree().ChangeScene("res://MainMenu.tscn");
				break;
		}
	}
}
