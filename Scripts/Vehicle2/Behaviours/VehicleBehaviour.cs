using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    public class VehicleBehaviour : MonoBehaviour
    {
        protected Rigidbody rb;
        protected MainController mc;
        
        public virtual void OnAwake()
        {            
            mc = GetComponent<MainController>();
        }

        public virtual void OnStart()
        {
            rb = GetComponent<Rigidbody>();
        }

        public virtual void BeforeOnUpdate()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void BeforeOnFixedUpdate()
        {

        }
        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

    }
}