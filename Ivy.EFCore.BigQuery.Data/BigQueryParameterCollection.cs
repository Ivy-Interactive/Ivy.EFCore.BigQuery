﻿using System.Collections;
using Google.Cloud.BigQuery.V2;
using System.Data.Common;

namespace Ivy.EFCore.BigQuery.Data;

public class BigQueryParameterCollection : DbParameterCollection
{
    private readonly List<BigQueryParameter> _parameters = new List<BigQueryParameter>();
    private readonly object _syncRoot = new object();

    public override int Count => _parameters.Count;
    public override object SyncRoot => _syncRoot;

    public BigQueryParameter this[string parameterName]
    {
        get => (BigQueryParameter)GetParameter(parameterName);
        set => SetParameter(parameterName, value);
    }

    public BigQueryParameter this[int index]
    {
        get => (BigQueryParameter)GetParameter(index);
        set => SetParameter(index, value);
    }

    public BigQueryParameter Add(BigQueryParameter value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        if (Contains(value.ParameterName)) throw new ArgumentException($"Parameter '{value.ParameterName}' already exists in the collection.");
        _parameters.Add(value);
        return value;
    }

    public override int Add(object value)
    {
        if (value is not BigQueryParameter parameter)
        {
            throw new ArgumentException($"Value must be of type {nameof(BigQueryParameter)}.", nameof(value));
        }

        Add(parameter);
        return Count - 1;
    }

    public BigQueryParameter AddWithValue(string parameterName, object value)
    {
        return Add(new BigQueryParameter(parameterName, value));
    }

    public BigQueryParameter Add(string parameterName, BigQueryDbType bqDbType)
    {
        return Add(new BigQueryParameter(parameterName, bqDbType));
    }

    public BigQueryParameter Add(string parameterName, BigQueryDbType bqDbType, object value)
    {
        return Add(new BigQueryParameter(parameterName, bqDbType, value));
    }

    public override void AddRange(Array values)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));

        foreach (var item in values)
        {
            Add(item);
        }
    }

    public void AddRange(IEnumerable<BigQueryParameter> values)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));

        foreach (var parameter in values)
        {
            Add(parameter);
        }
    }

    public override void Clear() => _parameters.Clear();

    public override bool Contains(object value)
    {
        return value is BigQueryParameter parameter && _parameters.Contains(parameter);
    }

    public override bool Contains(string value) => IndexOf(value) != -1;

    public override int IndexOf(object value)
    {
        if (value is not BigQueryParameter parameter) return -1;
        return _parameters.IndexOf(parameter);
    }

    public override int IndexOf(string parameterName)
    {
        // Normalize parameter name (ensure starts with @) for lookup
        var normalizedName = parameterName.StartsWith("@") ? parameterName : "@" + parameterName;
        for (var i = 0; i < _parameters.Count; i++)
        {
            if (string.Equals(_parameters[i].ParameterName, normalizedName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }

    public override void Insert(int index, object value)
    {
        if (value is not BigQueryParameter parameter)
        {
            throw new ArgumentException($"Value must be of type {nameof(BigQueryParameter)}.", nameof(value));
        }
        if (Contains(parameter.ParameterName)) throw new ArgumentException($"Parameter '{parameter.ParameterName}' already exists in the collection.");
        _parameters.Insert(index, parameter);
    }

    public override void Remove(object value)
    {
        if (value is not BigQueryParameter parameter) return;
        _parameters.Remove(parameter);
    }

    public override void RemoveAt(int index) => _parameters.RemoveAt(index);

    public override void RemoveAt(string parameterName)
    {
        var index = IndexOf(parameterName);
        if (index >= 0)
        {
            RemoveAt(index);
        }
    }

    protected override DbParameter GetParameter(int index)
    {
        if (index < 0 || index >= _parameters.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return _parameters[index];
    }

    protected override DbParameter GetParameter(string parameterName)
    {
        var index = IndexOf(parameterName);
        if (index < 0)
        {
            throw new IndexOutOfRangeException($"Parameter '{parameterName}' not found in the collection.");
        }
        return GetParameter(index);
    }

    protected override void SetParameter(int index, DbParameter value)
    {
        if (value is not BigQueryParameter parameter)
        {
            throw new ArgumentException($"Value must be of type {nameof(BigQueryParameter)}.", nameof(value));
        }
        if (index < 0 || index >= _parameters.Count)
        {
            throw new IndexOutOfRangeException();
        }
        var existingIndex = IndexOf(parameter.ParameterName);
        if (existingIndex >= 0 && existingIndex != index)
        {
            throw new ArgumentException($"Parameter '{parameter.ParameterName}' already exists in the collection at a different index.");
        }
        _parameters[index] = parameter;
    }

    protected override void SetParameter(string parameterName, DbParameter value)
    {
        if (value is not BigQueryParameter parameter)
        {
            throw new ArgumentException($"Value must be of type {nameof(BigQueryParameter)}.", nameof(value));
        }

        var index = IndexOf(parameterName);
        if (index < 0)
        {
            throw new IndexOutOfRangeException($"Parameter '{parameterName}' not found in the collection.");
        }

        var normalizedNewName = parameter.ParameterName.StartsWith("@") ? parameter.ParameterName : "@" + parameter.ParameterName;
        var normalizedOldName = parameterName.StartsWith("@") ? parameterName : "@" + parameterName;
        if (!string.Equals(normalizedNewName, normalizedOldName, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Cannot change parameter name when setting by name. Attempted to replace '{parameterName}' with '{parameter.ParameterName}'.");
        }
        SetParameter(index, parameter);
    }

    public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();

    public override void CopyTo(Array array, int index) => ((IList)_parameters).CopyTo(array, index);
    public void CopyTo(BigQueryParameter[] array, int index) => _parameters.CopyTo(array, index);

    internal IList<Google.Cloud.BigQuery.V2.BigQueryParameter> ToBigQueryParameters()
    {
        if (Count == 0)
        {
            return null;
        }

        var bqParams = new List<Google.Cloud.BigQuery.V2.BigQueryParameter>(Count);
        bqParams.AddRange(_parameters.Select(param => param.ToBigQueryParameter()));

        return bqParams;
    }
}
