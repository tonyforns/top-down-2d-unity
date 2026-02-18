using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCharacter : BaseCharacter
{
     [SerializeField]InputActionReference move;
   Animator animator;
   
   protected override void Awake()
   {
        base.Awake();
        animator = GetComponent<Animator>();
   }

   private void OnEnable()
   {
     move.action.Enable();//activa la escucha de la accion
     move.action.started += OnMove; //primera vez que se mueve el scripts
     move.action.performed += OnMove; //si lo cambio de posicion 
     move.action.canceled += OnMove; //cuando se suelte
   }
   
   
    protected override void Update()
    {
        base.Update();
        //Leer los inputs

        Move(rawMove); 
        //Responder a los inputs
    }

    private void OnDisable()
    {
     move.action.Disable();//desactiva la escucha de la accion
     move.action.started -= OnMove; 
     move.action.performed -= OnMove; 
     move.action.canceled -= OnMove; 
    }

    Vector2 rawMove;
     private void OnMove(InputAction.CallbackContext context)
     {
        rawMove = context.action.ReadValue<Vector2>(); //Context la Llamada / el valor que se leera uno lo designa , si tiene 2 ejes es vector 2 y si tiene un boton seria buton
     }

}

