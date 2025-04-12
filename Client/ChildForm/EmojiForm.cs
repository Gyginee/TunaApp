using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing; 
namespace Client
{
    public partial class EmojiForm : Form
    {
        public event Action<string> EmojiSelected;

        public EmojiForm()
        {
            InitializeComponent(); 
            InitializeEmojiButtons(); 

            this.Text = "Chọn Emoji";
            this.ClientSize = new System.Drawing.Size(350, 250); 
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; 
            this.StartPosition = FormStartPosition.CenterParent; 
        }

        private void InitializeEmojiButtons()
        {
       
            var emojis = new Dictionary<string, string>
            {
                {"😀", "grinning"}, {"😃", "smiley"}, {"😄", "smile"}, {"😁", "grin"}, {"😆", "laughing"}, {"😅", "sweat_smile"},
                {"😂", "joy"}, {"🤣", "rofl"}, {"😊", "blush"}, {"😇", "innocent"}, {"🙂", "slightly_smiling_face"}, {"🙃", "upside_down_face"},
                {"😉", "wink"}, {"😌", "relieved"}, {"😍", "heart_eyes"}, {"🥰", "smiling_face_with_hearts"}, {"😘", "kissing_heart"}, {"😗", "kissing"},
                {"😙", "kissing_smiling_eyes"}, {"😚", "kissing_closed_eyes"}, {"😋", "yum"}, {"😛", "stuck_out_tongue"}, {"😜", "stuck_out_tongue_winking_eye"},
                {"🤪", "zany_face"}, {"🤨", "face_with_raised_eyebrow"}, {"🧐", "monocle_face"}, {"🤓", "nerd_face"}, {"😎", "sunglasses"},

                {"🤩", "star_struck"}, {"🥳", "partying_face"}, {"😏", "smirk"}, {"😒", "unamused"}, {"😞", "disappointed"}, {"😔", "pensive"},
                {"😟", "worried"}, {"😕", "confused"}, {"🙁", "slightly_frowning_face"}, {"☹️", "frowning_face"}, {"😮", "open_mouth"}, {"😯", "hushed"},
                {"😲", "astonished"}, {"😳", "flushed"}, {"🥺", "pleading_face"}, {"😦", "frowning"}, {"😧", "anguished"}, {"😨", "fearful"},
                {"😰", "cold_sweat"}, {"😥", "sad_but_relieved_face"}, {"😢", "cry"}, {"😭", "sob"}, {"😱", "scream"}, {"😖", "confounded"},
                {"😣", "persevere"}, {"😓", "sweat"}, {"😩", "weary"}, {"😫", "tired_face"}, {"🥱", "yawning_face"},

                {"😤", "triumph"}, {"😠", "angry"}, {"😡", "rage"}, {"🤬", "face_with_symbols_on_mouth"}, {"🤯", "exploding_head"}, {"🥵", "hot_face"},
                {"🥶", "cold_face"}, {"🤔", "thinking"}, {"🤫", "shushing_face"}, {"🤭", "face_with_hand_over_mouth"}, {"🤗", "hugging_face"},
                {"🤞", "crossed_fingers"}, {" Hàng", "peace"}, {"🤘", "metal"}, {"🤙", "call_me_hand"}, {"👈", "point_left"}, {"👉", "point_right"},
                {"👆", "point_up"}, {"👇", "point_down"}, {"☝️", "point_up_2"}, {"👍", "+1"}, {"👎", "-1"}, {"✊", "fist"},
                {"👊", "punch"}, {"🤛", "left_fist"}, {"🤜", "right_fist"}, {"👏", "clap"}, {"🙌", "raised_hands"}, {"👐", "open_hands"},
                {"🤲", "palms_up_together"}, {"🤝", "handshake"}, {"🙏", "pray"}, {"✍️", "writing_hand"}, {"💅", "nail_care"}, {"🤳", "selfie"},

                {"❤️", "heart"}, {"🧡", "orange_heart"}, {"💛", "yellow_heart"}, {"💚", "green_heart"}, {"💙", "blue_heart"}, {"💜", "purple_heart"},
                {"🖤", "black_heart"}, {"💔", "broken_heart"}, {"❣️", "heavy_heart_exclamation"}, {"💕", "two_hearts"}, {"💞", "revolving_hearts"}, {"💓", "heartbeat"},
                {"💗", "growing_heart"}, {"💖", "sparkling_heart"}, {"💘", "cupid"}, {"💝", "gift_heart"}, {"💟", "heart_decoration"}, {"☮️", "peace_symbol"},
                {"✝️", "latin_cross"}, {"☪️", "star_and_crescent"}, {"🕉️", "om"}, {"☸️", "wheel_of_dharma"}, {"✡️", "star_of_david"}, {"☯️", "yin_yang"},
                {"☦️", "orthodox_cross"}, {"🛐", "place_of_worship"}, {"⛎", "ophiuchus"}, {"♈", "aries"}, {"♉", "taurus"}, {"♊", "gemini"},

                {"💯", "100"}, {"🔥", "fire"}, {"✨", "sparkles"}, {"⭐", "star"}, {"🌟", "star2"}, {"🎉", "tada"},
                {"🎊", "confetti_ball"}, {"🎈", "balloon"}, {"🎁", "gift"}, {"🎗️", "reminder_ribbon"}, {"🎟️", "tickets"}, {"🎫", "ticket"},
                {"☀️", "sun"}, {"🌙", "moon"}, {"☁️", "cloud"}, {"☂️", "umbrella"}, {"❄️", "snowflake"}, {"☃️", "snowman"},
                {"🐶", "dog"}, {"🐱", "cat"}, {"🐭", "mouse"}, {"🐹", "hamster"}, {"🐰", "rabbit"}, {"🦊", "fox"},
                {"❓", "question"}, {"❔", "grey_question"}, {"❕", "grey_exclamation"}, {"❗", "exclamation"}, {"⚠️", "warning"}
            };

            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, 
                AutoScroll = true,      
                FlowDirection = FlowDirection.LeftToRight, 
                WrapContents = true,     
                Padding = new Padding(5) 
            };

            var emojiFont = new Font("Segoe UI Emoji", 14);

            ToolTip toolTip = new ToolTip();

            foreach (var emojiPair in emojis)
            {
                var btn = new Button
                {
                    Text = emojiPair.Key,       
                    Tag = emojiPair.Key,        
                    Width = 45,                 
                    Height = 45,
                    Font = emojiFont,           
                    Margin = new Padding(3),    
                    FlatStyle = FlatStyle.Flat, 
                };
                btn.FlatAppearance.BorderSize = 0; 
                btn.FlatAppearance.MouseOverBackColor = Color.LightGray; 

                toolTip.SetToolTip(btn, emojiPair.Value); 

                btn.Click += (sender, e) =>
                {
                    EmojiSelected?.Invoke(btn.Tag.ToString());

                    this.Close();
                };

                flowLayout.Controls.Add(btn);
            }

            this.Controls.Add(flowLayout);
        }
    }
    
}
