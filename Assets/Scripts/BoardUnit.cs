using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BoardUnit : MonoBehaviour
{
    [SerializeField] public TMP_Text BoardUnitText;

    public int row;
    public int col;
    [SerializeField] public bool isOccupied = false;
    [SerializeField] public bool isAttacked = false;
    // Start is called before the first frame update
    void Start()
    {
        BoardUnitText.text = $"B[{row},{col}]";
    }
	public void GotAttacked(){
        this.isAttacked = true;
    }
}
