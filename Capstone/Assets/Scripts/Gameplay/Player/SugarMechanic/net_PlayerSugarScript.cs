using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
namespace ckp
{

    public class net_PlayerSugarScript : NetworkBehaviour
    {

        net_TeamScript tScript;
        int sugarLimit;
        int currentSugar = 0;
        int currentSugarStash = 0;

        public bool isDepositing = true;
        public bool isStealing = true;


        void Initialize()
        {
            tScript = GetComponent<net_TeamScript>();
            sugarLimit = net_GameManager.gs.maxSugarLimit;
        }

        public void DropSugar()
        {
            if (currentSugar < 1)
            {
                return;
            }

            currentSugar--;


        }

        public void AddSugar()
        {
            currentSugar++;
        }

        public void DepositSugar()
        {
            if (currentSugar < 1)
                return;

            currentSugar--;
            currentSugarStash++;
        }


        //Walk over sugar
        void CollectSugar(Collider other)
        {

            if (other.tag == "SugarCube")
            {

                if (currentSugar < sugarLimit)
                    return;

                other.GetComponent<net_SugarPickup>().CmdCollectMe(this.gameObject); 
            }

        }

        void OnTriggerEnter(Collider other)
        {

            CollectSugar(other);


            //if (other.tag == "Dropoff Point")
            //{

            //    if (currentSugar < 1)
            //        return;

            //    net_TeamScript otherTeamScript = other.GetComponent<net_TeamScript>();

            //    if (otherTeamScript)
            //    {
            //        if(tScript.teamColor == otherTeamScript.teamColor)
            //        {
            //            isDepositing = true;
            //            //Home base code



            //        }
            //        else
            //        {
            //            isStealing = true;
            //            //Stealing code
            //        }
            //    }
            //}
        }

        //void OnTriggerExit(Collider other)
        //{
        //    if (other.tag == "Dropoff Point")
        //    {

        //    }
        //}



        //Several drop animations below

        //    public void StunDropSugar()
        //    {
        //        if (sugarInBackpack.Count > 0)
        //        {
        //            int dropAmount;
        //            if (sugarInBackpack.Count == 1)
        //                dropAmount = 1;
        //            else
        //                dropAmount = sugarInBackpack.Count / 2;

        //            for (int x = 0; x < dropAmount; x++)
        //            {
        //                StartCoroutine(DropSugarAni());
        //            }
        //        }
        //    }

        //    private IEnumerator DropSugarAni()
        //    {
        //        DropSugar();
        //        int count = 0;
        //        GameObject sugar = sugarInBackpack[0];
        //        sugar.SetActive(true);
        //        sugarInBackpack.Remove(sugarInBackpack[0]);
        //        sugar.transform.parent = null;
        //        Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 1, sugar.transform.position.z);

        //        while (count < 10)
        //        {
        //            count++;
        //            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
        //            yield return null;
        //        }

        //        Vector3 randomDropLoc = Random.onUnitSphere * 1.5f;
        //        randomDropLoc.y = 1f;
        //        count = 0;

        //        while (sugar.transform.position != randomDropLoc)
        //        {
        //            count++;
        //            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, randomDropLoc, sugarPickupSpeed * 2);
        //            yield return null;
        //        }

        //        sugar.GetComponent<SimpleRotate>().enabled = true;
        //        sugar.GetComponent<BoxCollider>().enabled = true;
        //    }

        //    private IEnumerator StealSugarAni(GameObject dropoffPoint)
        //    {
        //        int count = 0;

        //        PlayerClass otherPlayer = GameManager.instance.GetPlayerFromDropoff(dropoffPoint);

        //        if (otherPlayer.currentPlayerScore > 0)
        //        {
        //            GameObject sugar = otherPlayer.dropoffPoint.transform.GetChild(0).gameObject;
        //            sugarInBackpack.Add(sugar);
        //            otherPlayer.LoseSugar(1);
        //            player.PickupSugar();
        //            Vector3 topPos = new Vector3(dropoffPoint.transform.position.x, dropoffPoint.transform.position.y + 2, dropoffPoint.transform.position.z);
        //            sugar.transform.parent = null;
        //            sugar.SetActive(true);

        //            while (count < 20)
        //            {
        //                count++;
        //                sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
        //                yield return null;
        //            }

        //            count = 0;

        //            while (count < 20)
        //            {
        //                count++;
        //                sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
        //                sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
        //                yield return null;
        //            }

        //            sugar.SetActive(false);
        //            sugar.transform.localScale = Vector3.one; //Reset sugar scale, might need to change later
        //            sugar.transform.parent = transform;

        //            if (otherPlayer.currentPlayerScore > 0 && runAnimation)
        //                StartCoroutine(StealSugarAni(dropoffPoint));
        //        }
        //    }

        //    private IEnumerator PickupSugarAni(GameObject sugar)
        //    {
        //        SugarManager.instance.EnableNewSugar(sugar);

        //        sugar.GetComponent<SimpleRotate>().enabled = false;
        //        sugar.GetComponent<BoxCollider>().enabled = false;
        //        player.PickupSugar();
        //        sugarInBackpack.Add(sugar);
        //        sugar.transform.parent = transform;
        //        Vector3 saveScale = sugar.transform.localScale;

        //        while (sugar.transform.position != transform.position)
        //        {
        //            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
        //            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, transform.position, sugarPickupSpeed);
        //            yield return null;
        //        }

        //        sugar.SetActive(false);
        //        sugar.transform.localScale = saveScale;
        //    }

        //    private IEnumerator DropoffSugarAni(GameObject dropoffPoint)
        //    {
        //        int count = 0;

        //        Vector3 saveScale = sugarInBackpack[0].transform.localScale;
        //        GameObject sugar = sugarInBackpack[0];
        //        sugarInBackpack.Remove(sugarInBackpack[0]);
        //        sugar.transform.parent = null;
        //        player.DropoffSugarInStash();
        //        sugar.SetActive(true);

        //        Vector3 topPos = new Vector3(sugar.transform.position.x, sugar.transform.position.y + 1, sugar.transform.position.z);

        //        while (count < 10)
        //        {
        //            count++;
        //            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, sugarPickupSpeed);
        //            yield return null;
        //        }

        //        count = 0;

        //        while (count < 10)
        //        {
        //            count++;
        //            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.zero, sugarPickupSpeed);
        //            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, dropoffPoint.transform.position, sugarPickupSpeed);
        //            yield return null;
        //        }

        //        sugar.SetActive(false);
        //        sugar.transform.localScale = saveScale;
        //        sugar.transform.parent = dropoffPoint.transform;

        //        yield return new WaitForSeconds(dropoffDelay);

        //        if (sugarInBackpack.Count > 0 && runAnimation)
        //            StartCoroutine(DropoffSugarAni(dropoffPoint));
        //    }

        //}
    }
}