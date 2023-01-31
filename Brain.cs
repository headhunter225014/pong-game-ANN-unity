using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class Brain : MonoBehaviour
{
    //info about the paddle and the ball
    public GameObject paddle;
    public GameObject ball;
    //can help to figure where the ball is and how fast is it travelling
    private Rigidbody2D brb;

    private float yvel;
    //max the paddle can go without leaving the range
    private float paddleMinY = 8.73f;
    private float paddleMaxY = 17.53f;

    private float paddleMaxSpeed = 15;

    public float numSaved = 0;
    public float numMissed = 0;
    
    /*
     * Inputs:
     * 1) Ball x
     * 2) Ball y
     * 3) Ball Velocity X
     * 4) Ball Velocity y
     * 5) Paddle X
     * 6) Paddle Y
     */
    
    /*
     * Output:
     * Paddle velocity Y
     */

    private ANN neuralNetwork;

    private void Start()
    {
        //6 inputs, 1 output, 1 hidden layer, 4 neurons in layer, 0.11 learning speed.
        neuralNetwork = new ANN(6, 1, 1, 4, 0.01);
        //info about the rigidbody
        brb = ball.GetComponent<Rigidbody2D>();
    }

    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
    {
        //lists for inouts and outputs
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        //balls x pos, balls y pos, balls x velocity, balls y velocity, paddle pos x, paddle pos y
        inputs.Add(bx);
        inputs.Add(by);
        inputs.Add(bvx);
        inputs.Add(bvy);
        inputs.Add(px);
        inputs.Add(py);
        //training expected desired output
        outputs.Add(pv);
        
        if (train)
            return (neuralNetwork.Train(inputs, outputs));
        else
            return (neuralNetwork.CalcOutput(inputs, outputs));

    }

    private void Update()
    {
        float posy = Mathf.Clamp(paddle.transform.position.y + (yvel * Time.deltaTime * paddleMaxSpeed),
            paddleMinY, paddleMaxY);
        
        paddle.transform.position = new Vector3(paddle.transform.position.x, posy, paddle.transform.position.z);

        List<double> output = new List<double>();
        int layerMask = 1 << 6;
        //raycasting
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position,
            brb.velocity, 1000, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "tops")
            {
                Vector3 reflection = Vector3.Reflect(brb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }

            //check if we hit collider or anything with it
            if (hit.collider != null && hit.collider.gameObject.tag == "backwall")
            {
                float dy = (hit.point.y - paddle.transform.position.y);

                //run with all the vars, 
                output = Run(ball.transform.position.x, ball.transform.position.y,
                    brb.velocity.x, brb.velocity.y,
                    paddle.transform.position.x,
                    paddle.transform.position.y,
                    dy, true);

                yvel = (float)output[0];
            }
        }
        else
            yvel = 0;

    }
}
