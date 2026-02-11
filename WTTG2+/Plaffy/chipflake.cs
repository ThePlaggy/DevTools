using System;
using DG.Tweening;
using UnityEngine;

namespace Plaffy
{
    public class chipflake : MonoBehaviour
    {
        // Einige sachen sind veraltet die mit /* */
        // Getaggt sind sollte bestenfalls bearbeitet werden
        private void Awake()
        {
            this.NotActiv = false;
            if (this.NotActiv)
            {
                return;
            }
            Debug.Log("chipflake Script Geladen");
            chipflake.Ins = this;
            GameManager.StageManager.ThreatsNowActivated += this.ThreatsAreActivated;
        }

        private void OnDestroy()
        {
            if (this.NotActiv)
            {
                return;
            }
            chipflake.Ins = null;
            GameManager.StageManager.ThreatsNowActivated -= this.ThreatsAreActivated;
        }

        private void Update()
        {
            if (this.NotActiv)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.AttemptSpawnChipflake();
            }
            if (LookUp.chipflakespawn)
            {
                this.AttemptSpawnChipflake();
            }
            if (StateManager.PlayerState == PLAYER_STATE.DESK && this.spawned)
            {
                Debug.Log("Chipflake soft despawn");
                this.Despawn(true);
            }
        }

        private void ThreatsAreActivated()
        {
            if (this.NotActiv)
            {
                return;
            }
            if (this.Activated)
            {
                this.Activated = true;
                Debug.Log("Chipflake activated");
                this.generateFireWindow(450f, 900f);
            }
        }

        private void generateFireWindow(float min, float max)
        {
            if (this.NotActiv)
            {
                return;
            }
            GameManager.TimeSlinger.FireTimer(global::UnityEngine.Random.Range(min, max), new Action(this.AttemptSpawnChipflake), 0);
        }

        public void AttemptSpawnChipflake()
        {
            if (this.NotActiv)
            {
                return;
            }
            if (this.spawned || StateManager.PlayerState != PLAYER_STATE.COMPUTER /* || EnemyManager.State != ENEMY_STATE.IDLE */ || EnvironmentManager.PowerState == POWER_STATE.OFF || StateManager.BeingHacked)
            {
                GameManager.TimeSlinger.FireTimer(30f, new Action(this.AttemptSpawnChipflake), 0);
                return;
            }
            Debug.Log("Chipflake ready to spawn");
            this.spawned = true;
            LookUp.chipflakespawn = false;
            this.SpawnChipflake();
        }

        private void SpawnChipflake()
        {
            if (this.NotActiv)
            {
                return;
            }
            Debug.Log("Chipflake spawned at the desk");
            this.chipflakeobj = global::UnityEngine.Object.Instantiate<GameObject>(CustomObjectLookUp.chipflake);
            this.chipflakeobj.name = "chipflake";
            this.chipflakeobj.transform.position = new Vector3(3.485f, 40.679f, -3.055f);
            this.chipflakeobj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            this.chipflakeobj.transform.rotation = Quaternion.Euler(0f, 180f, 90f);
            this.chipflakeAHO = this.chipflakeobj.AddComponent<AudioHubObject>();
            this.chipflakeAHO.PlaySound(CustomSoundLookUp.spawnsound);
            this.chipflakeAHO.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 10f).OnComplete(delegate
            {
                if (this.spawned)
                {
                    Debug.Log("Chipflake hard despawn");
                    this.Despawn(false);
                    this.GameOver();
                }
            });
        }

        private void Despawn(bool again)
        {
            if (this.NotActiv)
            {
                return;
            }
            if (this.chipflakeobj == null || this.chipflakeAHO == null)
            {
                Debug.Log("Chipflake Error");
            }
            this.spawned = false;
            if (again)
            {
                GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.wegsound);
            }
            global::UnityEngine.Object.Destroy(this.chipflakeobj);
            if (again)
            {
                this.generateFireWindow(30f, 35f);
            }
        }

        private void GameOver()
        {
            if (this.NotActiv)
            {
                return;
            }
            Debug.Log("Chipflake gameover");
            int num = global::UnityEngine.Random.Range(0, 3);
            if (num == 0)
            {
                HitmanComputerJumper.Ins.AddComputerJump();
                //EnemyManager.State = ENEMY_STATE.HITMAN;
                return;
            }
            if (num == 1)
            {
                EnemyManager.PoliceManager.triggerDevSwat();
                //EnemyManager.State = ENEMY_STATE.POILCE;
                return;
            }
            CultComputerJumper.Ins.AddLightsOffJump();
            //EnemyManager.State = ENEMY_STATE.CULT;
        }

        public static chipflake Ins;

        private GameObject chipflakeobj;

        private AudioHubObject chipflakeAHO;

        private bool Activated;

        private bool spawned;

        private bool NotActiv;
    }
}