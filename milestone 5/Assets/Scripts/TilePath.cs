using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : IComparable<TilePath>, IEnumerable<Tile>
{
    private List<Tile> _tiles = new List<Tile>();

    private int _weight;
    public int Weight
    {
        get
        {
            if (next != null)
                return _weight + next.Weight;
            return _weight;
        }
        private set { _weight = value; }
    }
    private TilePath next = null;

    public TilePath()
    {

    }

    public TilePath(TilePath toCopy)
    {
        Weight = toCopy.Weight;
        foreach (var item in toCopy.GetPath())
        {
            _tiles.Add(item);
        }
    }

    public void AddTilePath(TilePath path)
    {
        if (next == null)
            next = path;
        else
            next.AddTilePath(path);
    }

    public void AddTileToPath(Tile tile)
    {
        if (next != null)
            next.AddTileToPath(tile);
        else
        {
            _tiles.Add(new Tile(tile));
            Weight += tile.Weight;
        }
    }

    public Tile GetMostRecentTile()
    {
        if (next == null)
            return _tiles[_tiles.Count - 1];
        return next.GetMostRecentTile();
    }

    public int CompareTo(object obj)
    {
        return CompareTo(obj as TilePath);
    }

    public int CompareTo(TilePath other)
    {
        if (other == null)
        {
            throw new Exception("Incompatable compare types.");
        }
        return Weight.CompareTo(other.Weight);
    }

    public List<Tile> GetPath()
    {
        if (next == null)
            return _tiles;
        return _tiles.Concat(next.GetPath()).ToList();
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return ((IEnumerable<Tile>)GetPath()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Tile>)GetPath()).GetEnumerator();
    }
}

