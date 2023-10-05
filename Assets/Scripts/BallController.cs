using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{   public Rigidbody rb;
    public float speed=15;
    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;
    public int minSwipeRecognition=500;
    public Vector2 swipePosLastFrame;
    public Vector2 swipePosCurrentFrame;
    public Vector2 currentSwipe;
    private Color solveColor;
    public ParticleSystem collisionParticles;
    private bool hasCollided=false;
    private Renderer playerRenderer;
    public AudioClip hitSound;
    private AudioSource playerAudio;
    // Start is called before the first frame update
    void Start()
    {   playerRenderer=GetComponent<Renderer>();
        solveColor=Random.ColorHSV(0.5f, 1);
        var playerMaterial=playerRenderer.material;
        playerMaterial=new Material(playerMaterial);
        playerMaterial.color=solveColor;
        playerRenderer.material=playerMaterial;
        GetComponent<MeshRenderer>().material.color=solveColor;
        var particleMain=collisionParticles.main;
        particleMain.startColor=new ParticleSystem.MinMaxGradient(solveColor);
        playerAudio=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {   if(isTravelling){
            rb.velocity=speed*travelDirection;
        }
        Collider[] hitColliders=Physics.OverlapSphere(transform.position-(Vector3.up/2),0.05f);
        int i=0;
        while(i<hitColliders.Length){
            GroundPiece ground=hitColliders[i].transform.GetComponent<GroundPiece>();
            if(ground && !ground.isColored){
                ground.ChangeColor(solveColor);
            }
            i++;
        }
        if(nextCollisionPosition !=Vector3.zero){
            if(Vector3.Distance(transform.position,nextCollisionPosition)<1){
                isTravelling=false;
                travelDirection=Vector3.zero;
                nextCollisionPosition=Vector3.zero;
                collisionParticles.Play();
                hasCollided=true;
                playerAudio.PlayOneShot(hitSound,0.05f);
                //StartCoroutine(StopParticles());

            }
        }
        if(isTravelling){
            hasCollided=false;
            return;
        }
        if(Input.GetMouseButton(0)){
            swipePosCurrentFrame=new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            if(swipePosLastFrame!=Vector2.zero){
                currentSwipe=swipePosCurrentFrame-swipePosLastFrame;
                if(currentSwipe.sqrMagnitude<minSwipeRecognition){
                    return;
                }
                currentSwipe.Normalize();
                if(currentSwipe.x>-0.5f && currentSwipe.x<0.5){
                    //move up/down
                    SetDestination(currentSwipe.y>0?Vector3.forward : Vector3.back);
                }
                if(currentSwipe.y>-0.5f && currentSwipe.y<0.5){
                    //move left/right
                     SetDestination(currentSwipe.x>0?Vector3.right : Vector3.left);
                }
                
            }
            swipePosLastFrame=swipePosCurrentFrame;
        }
        if(Input.GetMouseButtonUp(0)){
            swipePosLastFrame=Vector2.zero;
            currentSwipe=Vector2.zero;
        }

        
    }
    private IEnumerator StopParticles(){
        yield return new WaitForSeconds(2.0f);
        collisionParticles.Stop();
    }
    private void SetDestination(Vector3 direction){
        travelDirection=direction;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,direction,out hit,100f)){
            nextCollisionPosition=hit.point;
        }
        isTravelling=true;
    }
}
