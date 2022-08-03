using System.IO;
using UnityEngine;

namespace GameSave
{
    public class GameDataWriter 
    {
    	private BinaryWriter writer;
    
        public GameDataWriter(BinaryWriter writer) 
        {
    		this.writer = writer;
    	}
        public void Write(int value)
        {
            writer.Write(value);
        }
        public void Write(float value)
        {
            writer.Write(value);
        }
        public void Write(Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
    }
}