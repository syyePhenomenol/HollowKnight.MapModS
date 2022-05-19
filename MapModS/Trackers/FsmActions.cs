using HutongGames.PlayMaker;
using MapModS.Data;
using UnityEngine;

namespace MapModS.Trackers
{
    public class TrackGeoRock : FsmStateAction
    {
        private readonly GameObject _go;
        private readonly GeoRockData _grd;

        public TrackGeoRock(GameObject go)
        {
            _go = go;
            _grd = _go.GetComponent<GeoRock>().geoRockData;
        }

        public override void OnEnter()
        {
            if (!_grd.id.Contains("-"))
            {
                MapModS.LS.ObtainedVanillaItems[_grd.id + _grd.sceneName] = true;
            }

            MapModS.LS.GeoRockCounter ++;

            //MapModS.Instance.Log("Geo Rock broken");
            //MapModS.Instance.Log(" ID: " + _grd.id);
            //MapModS.Instance.Log(" Scene: " + _grd.sceneName);

            Finish();
        }
    }

    public class TrackItem : FsmStateAction
    {
        private readonly string _oName;

        public TrackItem(string oName)
        {
            _oName = oName;
        }

        public override void OnEnter()
        {
            string scene = Utils.CurrentScene()??"";

            if (!_oName.Contains("-"))
            {
                MapModS.LS.ObtainedVanillaItems[_oName + scene] = true;
            }

            //MapModS.Instance.Log("Item picked up");
            //MapModS.Instance.Log(" Name: " + _oName);
            //MapModS.Instance.Log(" Scene: " + scene);

            Finish();
        }
    }
}