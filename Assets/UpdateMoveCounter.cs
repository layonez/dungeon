using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UpdateMoveCounter : MonoBehaviour {

	private Text text;
	public GameObject textgameobject;
	// Use this for initialization
	void Start () {
		text = textgameobject.GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {
		GameObject go = GameObject.Find ("Player");
		MovePlayer movePlayer = go.GetComponent <MovePlayer> ();
		text.text = movePlayer.Moves.ToString();
	}
}
