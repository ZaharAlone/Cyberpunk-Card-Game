using System.Collections.Generic;

namespace CyberNet.Core.City
{
    public static class CityStaticLogic
    {
        public static List<ConnectPointTypeGUID> SetConnectPointGUIDList(ConnectPointMono connectPoint)
        {
            var listPoints = new List<ConnectPointTypeGUID>();
            if (connectPoint.PreviousPoint.TypeCityPoint != TypeCityPoint.None)
                listPoints.Add(CreateConnectPointTypeGuid(connectPoint.PreviousPoint));
            if (connectPoint.NextPoint.TypeCityPoint != TypeCityPoint.None)
                listPoints.Add(CreateConnectPointTypeGuid(connectPoint.NextPoint));
            
            return listPoints;
        }

        private static ConnectPointTypeGUID CreateConnectPointTypeGuid(ConnectPointStruct connectPoint)
        {
            var connectPointResult = new ConnectPointTypeGUID();

            if (connectPoint.TypeCityPoint == TypeCityPoint.Tower)
            {
                connectPointResult.TypeCityPoint = TypeCityPoint.Tower;
                connectPointResult.GUIDPoint = connectPoint.TowerPoint.GUID;
            }
            else if (connectPoint.TypeCityPoint == TypeCityPoint.ConnectPoint)
            {
                connectPointResult.TypeCityPoint = TypeCityPoint.ConnectPoint;
                connectPointResult.GUIDPoint = connectPoint.ConnectPoint.GUID;
            }
            return connectPointResult;
        }
    }
}