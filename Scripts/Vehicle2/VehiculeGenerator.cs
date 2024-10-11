using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Vehicle;

class VehiculeGenerator
{
    public static GameObject GenerataPodDestroyable(PodModel _model, GameObject _parent = null, int _layer = -1)
    {
        _parent = _parent ?? new GameObject("ok");

        GeneratePodGameObject(_model, null, _layer, _parent.transform);
        GenerateFracturedPodGameObject(_model, null, _layer, _parent.transform);

        return _parent;
    }

    public static GameObject GeneratePodGameObject(PodModel _model, GameObject _base = null, int _layer = -1, Transform parent = null)
    {
        if (_base == null)
        {
            _base = GameObject.Instantiate(SavedDatasManager.GetPartByID(_model.IDBaseFrame).gameObject, parent);
            //_base.transform.localPosition = Vector3.zero;
            _base.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        foreach (KeyValuePair<int, List<float[]>> p in _model.Parts)
        {
            foreach (float[] fs in _model.Parts[p.Key])
            {
                Part part = SavedDatasManager.GetPartByID(p.Key);
                PutPart(part, _base);
                //GameObject g = GameObject.Instantiate(part.gameObject, _base.transform);
                //if (_layer != -1) g.layer = _layer;


                //g.transform.localPosition = new Vector3(fs[0], fs[1], fs[2]) - Vector3.up;
                //g.transform.localScale = new Vector3(fs[3], fs[4], fs[5]);
                //if (part.IsPair)
                //{
                //    foreach (PodPivot podPivot in _base.GetComponentsInChildren<PodPivot>())
                //    {
                //        if (podPivot.transform.position == g.transform.position)
                //        {
                //            podPivot.elementPut = g;
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (PivotSet pivotSet in _base.GetComponentsInChildren<PivotSet>())
                //    {
                //        if (pivotSet.transform.position == g.transform.position)
                //        {
                //            pivotSet.GetComponentsInChildren<PodPivot>().ToList().ForEach(x => x.elementPut = g);
                //        }
                //    }
                //}
            }
        }

        foreach (Transform trans in _base.GetComponentsInChildren<Transform>(true))
        {
            if (_layer > -1)
                trans.gameObject.layer = _layer;
        }

        return _base;
    }

    public static GameObject GenerateFracturedPodGameObject(PodModel _model, GameObject _base = null, int _layer = -1, Transform parent = null)
    {
        List<FracturedPart> fracturedParts = SavedDatasManager.FracturedParts;
        if (_base == null)
        {
            _base = GameObject.Instantiate(fracturedParts.Find(x => x.ID == _model.IDBaseFrame).gameObject, parent);
            //_base.transform.localPosition = Vector3.zero;
            _base.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        foreach (KeyValuePair<int, List<float[]>> p in _model.Parts)
        {
            foreach (float[] fs in _model.Parts[p.Key])
            {
                if (SavedDatasManager.AllParts.Find(x => x.ID == p.Key).TypePart != TypePart.Engine)
                {
                    FracturedPart fractPart = fracturedParts.Find(x => x.ID == p.Key);
                    GameObject g = GameObject.Instantiate(fractPart.gameObject, _base.transform);
                    if (_layer != -1) g.layer = _layer;

                    g.transform.localPosition = new Vector3(fs[0], fs[1], fs[2]) - Vector3.up;
                    g.transform.localScale = new Vector3(fs[3], fs[4], fs[5]);
                    if (fractPart.IsPair)
                    {
                        foreach (PodPivot podPivot in _base.GetComponentsInChildren<PodPivot>())
                        {
                            if (podPivot.transform.position == g.transform.position)
                            {
                                podPivot.elementPut = g;
                            }
                        }
                    }
                    else
                    {
                        foreach (PivotSet pivotSet in _base.GetComponentsInChildren<PivotSet>())
                        {
                            if (pivotSet.transform.position == g.transform.position)
                            {
                                pivotSet.GetComponentsInChildren<PodPivot>().ToList().ForEach(x => x.elementPut = g);
                            }
                        }
                    }
                }
            }
        }

        foreach (Transform trans in _base.GetComponentsInChildren<Transform>(true))
        {
            if (_layer > -1)
                trans.gameObject.layer = _layer;
        }
        return _base;
    }

    public static void PutPart(Part _p, GameObject _base)
    {
        PivotSet pivotSet = _base.GetComponentsInChildren<PivotSet>().ToList().Find(x => x.PartsAccepted == _p.TypePart);
        PodPivot[] pivots = pivotSet.PodPivots; 

        if (_p.IsPair)
        {
            for (int i = 0; i < pivots.Length; i++)
            {
                if (pivots[i].elementPut != null)
                    GameObject.Destroy(pivots[i].elementPut);
                GameObject g = GameObject.Instantiate(_p.gameObject, _base.transform);
                g.transform.position = pivots[i].transform.position;
                if (i == 0)
                    g.transform.localScale = new Vector3(-g.transform.localScale.x, g.transform.localScale.y, g.transform.localScale.z);
                pivots[i].elementPut = g;
            }
        }
        else
        {
            GameObject g = GameObject.Instantiate(_p.gameObject, _base.transform);
            for (int i = 0; i < pivots.Length; i++)
            {
                if (pivots[i].elementPut != null)
                    GameObject.Destroy(pivots[i].elementPut);
                pivots[i].elementPut = g;
            }
        }
    }
}
