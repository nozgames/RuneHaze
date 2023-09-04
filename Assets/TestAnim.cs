using System.Collections;
using System.Collections.Generic;
using NoZ.Animations;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    [SerializeField] private BlendedAnimationController _controller = null;
    [SerializeField] private AnimationShader _shader = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _controller.Play(_shader);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
