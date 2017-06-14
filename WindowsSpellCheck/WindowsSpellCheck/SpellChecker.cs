using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsSpellCheck
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpellError
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint Length;
        [MarshalAs(UnmanagedType.U4)]
        public uint Start;
        ///public string Replacement;
    }

    public class SpellChecker : IDisposable
    {
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Init();
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateSpellCheckerFactory();
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)] //ISpellCheckerFactory* factory, PCWSTR language
        private static extern IntPtr CreateSpellChecker(IntPtr factory, string language);
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint CheckSpelling(IntPtr checker, string str, IntPtr dest);
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetSuggestions(IntPtr checker, string str, IntPtr dest);
        [DllImport("SpellCheckNative.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint Release(IntPtr factory, IntPtr checker);

        IntPtr _factory;
        IntPtr _checker;


        public SpellChecker(string language)
        {
            Init();
            _factory = CreateSpellCheckerFactory();
            if (_factory == IntPtr.Zero)
                throw new Exception("Could not create SpellChecker factory");
            _checker = CreateSpellChecker(_factory, language);
            if (_factory == IntPtr.Zero)
                throw new Exception("Could not create SpellChecker");

        }

        public void Dispose()
        {
            Release(_factory, _checker);
        }

        public List<SpellError> GetErrors(string text)
        {
            List<SpellError> errors = new List<SpellError>();

            IntPtr dest = Marshal.AllocHGlobal(Marshal.SizeOf<SpellError>() * 50);

            uint count = CheckSpelling(_checker, text, dest);

            for(int i = 0; i < count; i++)
            {
                SpellError error = Marshal.PtrToStructure<SpellError>(dest + i * Marshal.SizeOf<SpellError>());
                errors.Add(error);
            }

            Marshal.FreeHGlobal(dest);

            return errors;
        }
        
        public List<string> GetSuggestions(string word)
        {
            List<string> suggestions = new List<string>();
            IntPtr dest = Marshal.AllocHGlobal(20 * (word.Length + 4)*4);
            uint count = GetSuggestions(_checker, word, dest);

            for(int i = 0; i < count; i++)
            {
                IntPtr ptr = Marshal.ReadIntPtr(dest + i * (word.Length + 4) * 4);
                string val = Marshal.PtrToStringUni(ptr)?.Trim();
                suggestions.Add(val);
            }

            Marshal.FreeHGlobal(dest);

            return suggestions;
        }
    }
}
