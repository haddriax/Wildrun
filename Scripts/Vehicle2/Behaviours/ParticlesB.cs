using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Vehicle
{
    public class ParticlesB : VehicleBehaviour
    {
        public enum FxName
        {
            reactorFlames,
            reactorDistortion,
            trails
        };

        public Dictionary<string, ParticleSystem[]> particlesSystems;

        public override void OnStart()
        {
            base.OnStart();

            particlesSystems = new Dictionary<string, ParticleSystem[]>();

            ParticleSystem[] p = transform.parent.GetComponentsInChildren<ParticleSystem>();


            List<ParticleSystem> reactorFlames = new List<ParticleSystem>();
            List<ParticleSystem> reactorDistortion = new List<ParticleSystem>();
            List<ParticleSystem> trails = new List<ParticleSystem>();

            foreach (ParticleSystem ps in p)
            {
                string psName = ps.name;
                if (psName.Contains("Flame") || psName.Contains("fire") || psName.Contains("Fire"))
                {
                    reactorFlames.Add(ps);
                    Debug.Log("Added particles : " + psName + " in reactorFlames ");
                }
                else if (psName.Contains("heat") || psName.Contains("distortion"))
                {
                    reactorDistortion.Add(ps);
                    Debug.Log("Added particles : " + psName + " in reactorDistortion");
                }
                else if (psName.Contains("Dust") || psName.Contains("dust"))
                {
                    trails.Add(ps);
                    Debug.Log("Added particles : " + psName + " in trails");
                }
            }

            particlesSystems.Add("reactorFlames", reactorFlames.ToArray());
            particlesSystems.Add("reactorDistortion", reactorDistortion.ToArray());
            particlesSystems.Add("trails", trails.ToArray());

        }

        public override void OnUpdate()
        {
            /*
            base.OnUpdate();
            particlesSystems.TryGetValue("reactorFlames", out ParticleSystem[] p);

            foreach (var item in p)
            {
                p.
            }
            */

        }

        public void SwitchOnOff(FxName fxName, bool on)
        {
            if (particlesSystems.TryGetValue(fxName.ToString(), out ParticleSystem[] p))
            {
                foreach (var item in p)
                {
                    if (on)
                        item.Play();
                    else
                        item.Pause();
                }
            }
        }

        public void SwitchAllOnOff(bool on)
        {
            foreach (FxName fxName in (FxName[])System.Enum.GetValues(typeof(FxName)) )
            {
                SwitchOnOff(fxName, on);
            }           
        }
    }



}