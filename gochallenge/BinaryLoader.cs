using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace gochallenge
{
    public class BinaryLoader
    {
        private string _file;
        private byte[] _data;
        private byte _datalength;

        private byte[] _header;
        private byte[] _version;
        private byte[] _tempo;
       

        public BinaryLoader(string PatternFile)
        {
            if (!File.Exists(PatternFile))
            {
                throw new Exception("File does not exist.");
            }
            this._file = PatternFile;
            this._data = new byte[0];
        }

        public bool Open()
        {
            using(FileStream fs = new FileStream(this._file, FileMode.Open))
            using (BinaryReader r = new BinaryReader(fs))
            {
                int x = 0;
                int bufsize = 8096;
                byte[] buffer = new byte[bufsize];

                while (x < r.BaseStream.Length)
                {
                    x += r.Read(buffer, x, bufsize);

                    _data = _data.Concat(buffer).ToArray();
                }

                _data = _data.Take(x).ToArray();
                r.Close();

                return true;
            }
        }

        public Sample Parse()
        {
            _header = _data.Skip(0).Take(6).ToArray();            
            _datalength = _data.Skip(13).Take(1).ToArray()[0];
            _version = _data.Skip(14).Take(11).ToArray();
            _tempo = _data.Skip(0x2e).Take(4).ToArray();

            Sample resultSample = new Sample(_file);
            resultSample.AddHeader(_header);
            resultSample.AddVersion(_version);
            resultSample.AddTempo(_tempo);

            
            // parse tracks
            int pos = 0x32;
            int TrackDataLength = Convert.ToInt32(_datalength) - 40;

            while (pos < _data.Length)
            {
                
                byte[] Id = _data.Skip(pos).Take(4).ToArray();
                int TrackNameLength = Convert.ToInt32(_data.Skip(pos + 4).Take(1).ToArray()[0]);
                byte[] TrackName = _data.Skip(pos + 4 + 1).Take(TrackNameLength).ToArray();
                byte[] Measures = _data.Skip(pos + 4 + 1 + TrackNameLength).Take(4 * 4).ToArray();

                if (Measures.Length != 16)
                    break;
                Track t = new Track();
                t.AddId(Id);
                t.AddName(TrackName);
                t.AddMeasures(Measures);

                resultSample.AddTrack(t);

                pos += 4 + 1 + TrackNameLength + 16;
            }

            return resultSample;
        }
    }
}
