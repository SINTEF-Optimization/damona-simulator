using Damona.Simulator;

namespace Damona.Schemas.AutoMapper;

/// <summary>
///     A map between specialties as strings and their internal integer ID.
/// </summary>
/// <param name="emergencyName">Name of the special emergency category</param>
/// <remarks>
///     In the simulation, specialties are single integers for performance reasons.
/// </remarks>
public class SpecialtyMap(string emergencyName = "emergency")
{
    private readonly List<MapItem> _maps = [new(Constants.EmergencyId, emergencyName)];
    private int _nextId = 0;

    /// <summary>
    ///     Change the name of the special emergency category.
    /// </summary>
    /// <param name="emergencyName">The new name of the emergency category.</param>
    public void SetEmergencyName(string emergencyName)
    {
        _maps.Remove(_maps.First(x => x.Id == Constants.EmergencyId));
        _maps.Add(new(Constants.EmergencyId, emergencyName));
    }

    /// <summary>
    ///     Get the integer ID of a specialty, add it to the map if it doesn't exist
    /// </summary>
    /// <param name="name">The name of the specialty</param>
    /// <returns>The ID of the specialty, or the next available ID if it doesn't exist.</returns>
    public int GetOrAdd(string name)
    {
        var item = _maps.FirstOrDefault(x => x.Name == name);
        if (item is not null)
            return item.Id;

        var id = _nextId;
        Add(id, name);
        return id;
    }

    /// <summary>
    ///     Get the name of the specialty with the given ID.
    /// </summary>
    public string Get(int id)
    {
        return _maps.First(x => x.Id == id).Name;
    }

    /// <summary>
    ///     Get the ID of a specialty with the given name. Same as <see cref="GetOrAdd"/>, except that it fails if the
    ///     ID hasn't been created.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the ID doesn't exist in the map.</exception>
    public int GetId(string name)
    {
        return _maps.First(x => x.Name == name).Id;
    }

    private void Add(int id, string name)
    {
        if (_maps.Any(x => x.Id == id && x.Name == name))
            return;

        if (_maps.Any(x => x.Id == id))
            throw new ArgumentException($"Id {id} already exists in the map");

        if (_maps.Any(x => x.Name == name))
            throw new ArgumentException($"Name {name} already exists in the map");

        _nextId = id + 1;
        _maps.Add(new(id, name));
    }

    private record MapItem(int Id, string Name);
}