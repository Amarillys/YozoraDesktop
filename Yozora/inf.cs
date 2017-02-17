using System;

namespace Yozora
{
    public class Inf
    {
        public Inf() { vint_ = code_ = -1; vstr_ = detail_ = "Undealed"; show(); }
        public Inf(Int32 p_code, String p_string) { code_ = p_code; detail_ = p_string; show(); }
        public Inf(Int32 p_code, String p_string, Int32 p_int) { code_ = p_code; detail_ = p_string; vint_ = p_int; show(); }
        public Inf(Int32 p_code, String p_string, Int32 p_int, String p_str) { code_ = p_code; detail_ = p_string; vint_ = p_int; vstr_ = p_str; show(); }
        public Inf(Int32 p_code, String p_string, String p_str) { code_ = p_code; detail_ = p_string; vstr_ = p_str; show(); }

        void show()
        {
            Console.WriteLine("code : " + code_ + "; detail : " + detail_);
        }

        private Int32 code_;
        private String detail_;
        private Int32 vint_;
        private String vstr_;
    };
}
