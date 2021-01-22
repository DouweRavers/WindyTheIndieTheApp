using Godot;
using System;
using System.Collections.Generic;

public class Map : TileMap
{
	//control
	public int size;
	
	//local variables
	private List<Tree> trees;
	private double treeHealth;
	
	//references
	Player player;
	
	//children
	YSort ysort;
	PackedScene treeResource;
	
	public override void _Ready(){
		ysort = (YSort) GetNode("YSort");
		treeResource = (PackedScene) ResourceLoader.Load("res://Prefab/Objects/Tree.tscn");
		trees = new List<Tree>();
	}
	
	public void Init(Player p, double th){
		player = p;
		treeHealth = th;
		foreach(Node2D node in GetChildren()){
			if(node.Name != "noSort" && node.Name != "YSort"){
				node.GetParent().RemoveChild(node);
				ysort.AddChild(node);
			}
		}
	}
	
	public void DrawBase(int size){
		this.size = size;
		Random rand = new Random();
		for(int x = 0; x < size; x++){
			for(int y = 0; y < size; y++){
				if((y == size-1) || (y == 0) || (x == 0) || (x == size-1)){
					SetCell(x, y, 1);
				} else{
					SetCell(x, y, rand.Next(2,8));
				}
			}
		}
	}
	
	public bool PlaceTree(int x, int y, String phase = "Sapling"){
		bool ans = false;
		bool isoccupied = false;
		foreach(Tree tree in trees){
			if(x == tree.x && y == tree.y)
				isoccupied = true;
		}
		if( 0 < x && x < size-1 && 1 < y && y < size-1 && !isoccupied){
			Vector2 Wloc = MapToWorld(new Vector2(x ,y));
			Random rand = new Random();
			Wloc += new Vector2(rand.Next(64), rand.Next(64));
			Tree tree = (Tree) treeResource.Instance();
			ysort.AddChild(tree);
			tree.Position = Wloc;
			trees.Add(tree);
			tree.Init(x, y, treeHealth, this, player);
			tree.OverrideGrowPhase(phase);
			ans = true;
		}
		return ans;
	}
	
	public Tree getClosestTree(Vector2 vec){
		Tree closestTree = null;
		float closestLenght = 0;
		foreach(Tree tree in trees){
			if(!tree.burning && (closestTree is null || tree.Position.DistanceTo(vec) < closestLenght)){
				closestTree = tree;
				closestLenght = tree.Position.DistanceTo(vec);
			}
		}
		return closestTree;
	}
	
	public void removeTree(Tree tree){
		trees.Remove(tree);
		tree.QueueFree();
	}
	
	public int GetTreeCount(){
		return trees.Count;
	}
}
