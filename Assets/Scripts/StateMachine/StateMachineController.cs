using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class StateMachineController : MonoBehaviour
{
    // Start is called before the first frame update
    public static StateMachineController instance;
    State _current;
    bool busy;
    public Player player1; //gold
    public Player player2; //green
    public Player currentlyPlaying;
    public TaskCompletionSource<object> taskHold;
    public GameObject promotionPanel;
    
    void Awake(){
        instance = this;
    }

    void Start(){
        ChangeTo<LoadState>();
    }

    public void ChangeTo<T>() where T:State{
        State state = GetState<T>( );
        if(_current != state){
            ChangeState(state);
        }
    }

    public T GetState<T>() where T:State{
        T target = GetComponent<T>();
        if(target == null)
            target = gameObject.AddComponent<T>();
        return target;
        
    }

    void ChangeState(State value){
        if (busy) 
            return;
        busy = true;

        if(_current != null)
            _current.Exit();
        _current = value;

        if (_current != null)
            _current.Enter();

        busy = false;
    }
}
