using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{   
    private static MusicManager instance;
    private void Awake(){
        if(instance!=null && instance!=this){
            Destroy(this.gameObject);
            return;
        }
        instance=this;
        DontDestroyOnLoad(this.gameObject);
    }
}
