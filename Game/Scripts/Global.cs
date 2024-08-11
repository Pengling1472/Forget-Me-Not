using Godot;
using System;

public partial class Global : Node {

    public Godot.Collections.Dictionary Data { get; set; } = new Godot.Collections.Dictionary() {

        { "dialogues", new Godot.Collections.Dictionary() }

    };

    public Godot.Collections.Dictionary Settings { get; set; } = new Godot.Collections.Dictionary() {

        { "audio", new Godot.Collections.Dictionary() {

            { "music", ( int ) 50 },
            { "sound", ( int ) 50 }

        } },
        { "inputs", new Godot.Collections.Dictionary() {

            

        } },
        { "display", new Godot.Collections.Dictionary() {

            { "fullscreen", ( bool ) false },
            { "windowscale", ( int ) 1 } 

        } },

    };

    public void save( Json data ) {

        var file = FileAccess.OpenEncryptedWithPass( "user://file01", FileAccess.ModeFlags.Write, "PleaseFuckOff" );

        file.StoreString( data.ToString() );

        file.Close();

    }

}