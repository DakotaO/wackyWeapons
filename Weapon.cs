using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    public class Weapon : MonoBehaviour
    {

        //PUBLIC _______________________________________________________________

        //General

        /// <summary>
        /// spawnpoint of the bullet visualisation
        /// </summary>
        public Transform VisualSpawnPoint;
        /// <summary>
        /// spawnpoint of the real / physical bullet (usually center of the screen)
        /// </summary>
        public Transform PhysicalBulletSpawnPoint;

        /// <summary>
        /// Lifetime of each bullet
        /// </summary>
        public float LifeTimeOfBullets = 6;
        /// <summary>
        /// Layers that get detected by Raycast
        /// </summary>
        public LayerMask HitMask = new LayerMask();

        //Bullet
        /// <summary>
        /// damage the bullet deals at initialisation
        /// </summary>
        public float MuzzleDamage = 80;
        /// <summary>
        /// initial speed of the bullet
        /// </summary>
        public float MaxBulletSpeed = 550;
        /// <summary>
        /// randomisation of bullet speed
        /// </summary>
        public float randomSpeedOffset = 10;
        /// <summary>
        /// mass of each bullet
        /// </summary>
        public float BulletMass = 0.0065f;
        /// <summary>
        /// bullet diameter
        /// </summary>
        public float Diameter = 0.01f;
        /// <summary>
        /// the drag coefficient ( sphere .5f )
        /// </summary>
        public float DragCoefficient = 0.4f;
        /// <summary>
        /// Prefab of the bullet visuals
        /// </summary>
        public Transform BulletPref;
        //--

        //-----------------------------------------------------------------------

        //PRIVATE________________________________________________________________
        BallisticSettings Settings;

        private BulletHandler bulletHandler;

        //Pool
        private PoolManager myPool;

        //store precalculated drag to save performance
        public float PreDrag;
        private float area;

        //-----------------------------------------------------------------------

        void Awake()
        {

            myPool = PoolManager.instance;

            bulletHandler = BulletHandler.instance;
            if (bulletHandler == null) return;

            BallisticSettings bs = bulletHandler.Settings;
            if (bs != null)
            {
                Settings = bs;
            }

            RecalculatePrecalculatedValues();
        }

        //PUBLIC FUNCTIONS____________________________________________________________________________________________

        public BallisticSettings getSettings()
        {
            return Settings;
        }

        /// <summary>
        /// calculates mostly unchanged Values
        /// </summary>
        public void RecalculatePrecalculatedValues()
        {
            area = Mathf.Pow(Diameter / 2, 2) * Mathf.PI;
            if (Settings != null)
            {
                PreDrag = (0.5f * Settings.AirDensity * area * DragCoefficient) / BulletMass;
            }
        }

        //public functions ____________________________________________________________________________________

        /// <summary>
        /// calculate bulletdrop at given distance
        /// </summary>
        /// <param name="dist">distance</param>
        /// <param name="useDrag">bullet drag enabled</param>
        /// <param name="airDensity">air density</param>
        /// <returns></returns>
        public float calculateBulletdrop(float dist, bool useDrag, float airDensity = 1.22f)
        {
            float FlightTime;

            if (useDrag)
            {
                float k = (airDensity * DragCoefficient * Mathf.PI * (Diameter * .5f) * (Diameter * .5f)) / (2 * BulletMass);
                FlightTime = (Mathf.Exp(k * dist) - 1) / (k * MaxBulletSpeed);
            }
            else
            {
                FlightTime = dist / MaxBulletSpeed;
            }

            return .5f * -Physics.gravity.y * Mathf.Pow(FlightTime, 2);
        }

        /// <summary>
        /// calculate angle to counteract bullet drop at given distance
        /// </summary>
        /// <param name="dist">distance</param>
        /// <param name="useDrag">bullet drag enabled</param>
        /// <param name="airDensity">air density</param>
        /// <returns></returns>
        public float calculateZeroingCorrectionAngle(float dist, bool useDrag, float airDensity = 1.22f)
        {
            float drop = calculateBulletdrop(dist, useDrag, airDensity);
            return 360f - Mathf.Atan(drop / dist) * Mathf.Rad2Deg;
        }

        //-----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Instantiates the Bullet and gives them over to BulletHandler for Calculation
        /// </summary>
        /// <param name="ShootDirection">the direction the bullet is fired in ( usually you want to use 'PhysicalBulletSpawnPoint.forward' + some offset for recoil )</param>
        /// <param name="zeroAngle">to counteract bullet drop you can angle the bullet slighty upwards. You can calculate this angle with 'weapon.calculateZeroingCorrectionAngle(..)'</param>
        public void ShootBullet(Vector3 ShootDirection, float zeroAngle = 0)
        {
            Transform bClone = null;
            if (BulletPref != null)
            {
                GameObject cGO = myPool.GetNextGameObject(BulletPref.gameObject);
                if (cGO == null)
                {
                    bClone = (Transform)Instantiate(BulletPref, VisualSpawnPoint.position, Quaternion.identity);
                    bClone.SetParent(myPool.transform);
                }
                else
                {
                    cGO.SetActive(true);
                    bClone = cGO.transform;
                    
                }
                bClone.position = VisualSpawnPoint.position;
            }
            //calculte in zeroing corrections
            Vector3 dir = Quaternion.AngleAxis(zeroAngle, PhysicalBulletSpawnPoint.right) * ShootDirection;

            //give the BulletInstace over to the BulletHandler
            bulletHandler.AddBullet(new BulletData(this, PhysicalBulletSpawnPoint.position, VisualSpawnPoint.position - PhysicalBulletSpawnPoint.position, dir, LifeTimeOfBullets, randomSpeedOffset == 0 ? MaxBulletSpeed : MaxBulletSpeed + UnityEngine.Random.Range(0f, randomSpeedOffset) - randomSpeedOffset * .5f, bClone));

            //1 frame delayed - e.g. when working with trailrenderes 
            //StartCoroutine("shootDelayed", new BulletData(this, PhysicalBulletSpawnPoint.position, VisualSpawnPoint.position - PhysicalBulletSpawnPoint.position, dir, LifeTimeOfBullets, randomSpeedOffset == 0 ? MaxBulletSpeed : MaxBulletSpeed + UnityEngine.Random.Range(0f, randomSpeedOffset) - randomSpeedOffset / 2f, bClone));
        }

        //process bullet next frame
        /*IEnumerator shootDelayed(BulletData bullet)
        {
            yield return null;
            bulletHandler.AddBullet(bullet);

        }*/
    }
}