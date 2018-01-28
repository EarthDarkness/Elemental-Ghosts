using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour {
	public IconControl _ctrl = null;
	public int _x = 0;
	public int _y = 0;

	public int _sign = 0;

	public void Setup(int x, int y, int sign) {
		_x = x;
		_y = y;
		_sign = sign;
	}

	void OnMouseDown() {
		//Debug.Log("clicked");
		if(_sign == 0) {
			_ctrl.Set(_x, _y);
		}else if(_sign < 0) {
			_ctrl.Left();
		}else if(_sign > 0) {
			_ctrl.Right();
		}
	}

}
