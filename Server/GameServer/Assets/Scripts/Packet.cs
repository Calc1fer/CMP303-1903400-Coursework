using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    welcome = 1,
    spawn_player,
    player_pos,
    player_rot,
    player_disconnect,
    player_health,
    player_respawn,
    item_spawner,
    item_spawned,
    item_pickedup,
    spawn_projectile,
    projectile_destroy,
    projectile_pos,
    projectile_rot,
    spin_obstacle_spawner,
    spin_obstacle_rot,
    level_trigger_spawn,
    level_won,
    static_obstacle_spawner,
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    player_movement,
    player_shoot,
    interpolate_pos
}

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    //Create a new packet
    public Packet()
    {
        buffer = new List<byte>();
        readPos = 0;
    }

    //creates a packet with an id
    public Packet(int _id)
    {
        buffer = new List<byte>(); 
        readPos = 0;

        Write(_id); 
    }

    //creates a packet from which data can be read from
    public Packet(byte[] _data)
    {
        buffer = new List<byte>();
        readPos = 0;

        SetBytes(_data);
    }

    #region Functions

    //Prepares the packet to be read
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }

    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }

    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public int Length()
    {
        return buffer.Count; // Return the length of buffer
    }

    public int UnreadLength()
    {
        return Length() - readPos; // Return the remaining length (unread)
    }

    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear(); // Clear buffer
            readableBuffer = null;
            readPos = 0; // Reset readPos
        }
        else
        {
            readPos -= 4; // "Unread" the last read int
        }
    }
    #endregion

    #region Write Data
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }

    //adds an int to the packet
    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    //adds a float to the packet
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    //adds a bool to the packet
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    //adds a string to the packet
    public void Write(string _value)
    {
        Write(_value.Length); // Add the length of the string to the packet
        buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
    }

    //writes a vector to the packet
    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }

    //adds a quaternion to the packet
    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }


    #endregion

    #region Read Data

    //reads an array of bytes from the packet
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += _length; // Increase readPos by _length
            }
            return _value; // Return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    //reads an int
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4
            }
            return _value; // Return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    //reads a float
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4
            }
            return _value; // Return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    //reads a bool from the packet
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 1; // Increase readPos by 1
            }
            return _value; // Return the bool
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    //reads a string from the packet
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
            if (_moveReadPos && _value.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return _value; // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }
    #endregion

    //reads a vector3 from the packet
    public Vector3 ReadVec3(bool move_read_pos = true)
    {
        return new Vector3(ReadFloat(move_read_pos), ReadFloat(move_read_pos), ReadFloat(move_read_pos));
    }

    //reads a vector 4 from the packet
    public Vector3 ReadVec4(bool move_read_pos = true)
    {
        return new Vector4(ReadFloat(move_read_pos), ReadFloat(move_read_pos), ReadFloat(move_read_pos), ReadFloat(move_read_pos));
    }

    //reads a quaternion from the packet
    public Quaternion ReadQuaternion(bool move_read_pos = true)
    {
        return new Quaternion(ReadFloat(move_read_pos), ReadFloat(move_read_pos), ReadFloat(move_read_pos), ReadFloat(move_read_pos));
    }

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}