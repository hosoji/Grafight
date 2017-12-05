using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.WrappingRope.Contracts
{
    public interface IPolygonProvider
    {
        List<Vector3> GetPolygon();
    }
}
