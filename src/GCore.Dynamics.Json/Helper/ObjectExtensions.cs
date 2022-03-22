using System;
using System.Text.Json;

namespace GCore.Dynamics.Json.Helper;

public static class ObjectExtensions
{

    // https://stackoverflow.com/a/1750024
    public static bool IsNumericType(this object self)
    {
        switch (Type.GetTypeCode(self.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }


}