﻿namespace Rezoom.ORM
open System
open System.Collections.Generic

type ColumnType =
    | Invalid  = 0s
    | Object   = 1s // whatever it is goes through boxing
    | String   = 2s
    | Byte     = 3s
    | Int16    = 4s
    | Int32    = 5s
    | Int64    = 6s
    | SByte    = 7s
    | UInt16   = 8s
    | UInt32   = 9s
    | UInt64   = 10s
    | Single   = 11s
    | Double   = 12s
    | Decimal  = 13s
    | DateTime = 14s

type ColumnInfo =
    struct
        val Index : int16
        val Type : ColumnType
        new (index, rowValueType) = { Index = index; Type = rowValueType }
    end

[<AllowNullLiteral>]
type ColumnMap() =
    let columns = new Dictionary<string, ColumnInfo>(StringComparer.OrdinalIgnoreCase)
    let subMaps = new Dictionary<string, ColumnMap>(StringComparer.OrdinalIgnoreCase)
    static let columnMethod = typeof<ColumnMap>.GetMethod("Column")
    static let subMapMethod = typeof<ColumnMap>.GetMethod("SubMap")
    member private this.GetOrCreateSubMap(name) =
        let succ, sub = subMaps.TryGetValue(name)
        if succ then sub else
        let sub = new ColumnMap()
        subMaps.[name] <- sub
        sub
    member private this.SetColumn(name, info) =
        columns.[name] <- info
    member private this.Load(columnNames : (string * ColumnType) array) =
        let root = this
        let mutable current = this
        for i = 0 to columnNames.Length - 1 do
            let name, rowValueType = columnNames.[i]
            let path = name.Split('.', '$')
            if path.Length > 1 then
                current <- root
                for j = 0 to path.Length - 2 do
                    current <- current.GetOrCreateSubMap(path.[j])
            current.SetColumn(Array.last path, ColumnInfo(int16 i, rowValueType))
    member this.Column(name) =
        let succ, info = columns.TryGetValue(name)
        if succ then info else ColumnInfo(-1s, ColumnType.Invalid)
    member this.SubMap(name) =
        let succ, map = subMaps.TryGetValue(name)
        if succ then map else null
    static member Parse(columnNames) =
        let map = new ColumnMap()
        map.Load(columnNames)
        map

    static member internal ColumnMethod = columnMethod
    static member internal SubMapMethod = subMapMethod
