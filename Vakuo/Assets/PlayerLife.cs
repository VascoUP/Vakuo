using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerLife : MonoBehaviour {

    private int hearts = 5;
    private double lifes;
	PlayerLife () {
        this.lifes = 4 * this.hearts;
	}

    void CreateLifeCanvas(){
        for (int i = 0; i < this.hearts; i++)
        {

        }
    }

	double GetLifes(){
        return this.lifes;
    }
	
	void RemoveLifes() {
        this.lifes -= 0.5;

        if(Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
            this.hearts--;

        //remover vida de ultimo coração
	}

    void Addlifes(){
        if (this.hearts < 5){
            this.hearts++;

            this.lifes = 4 * this.hearts;

            //adicionar vidas após ultimo coração
        }
    }
}
