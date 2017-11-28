using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tool_uGUI_TextureAnimation : MonoBehaviour {

	public bool loop = true;
	public float frequency = 0.05f;
	public List<Texture> textures = new List<Texture>();	
	private RawImage img;
	private int index = 0;

	void Start()
	{
		RawImage[] imgs = this.GetComponents<RawImage> ();
		this.img = imgs [0];
		this.start_ainimaton();
	}
	
	void start_ainimaton()
	{
		this.index = 0;
		this.StartCoroutine(this.update_ainimaton());
	}
	
	IEnumerator update_ainimaton()
	{
		
		while (true)
		{
			if (this.textures[this.index] != null)
				this.img.texture = this.textures[this.index];//
			this.index++;
			yield return new WaitForSeconds(this.frequency);
			
			if (this.index >= this.textures.Count)
			{
				if (this.loop)
					this.index = 0;
				else
					break;
			}
			
		}
	}
}
