using GTA;
using GTA.Native;
using GTA.Math;
using System;

public class ComidaEBebida : Script
{
    private Prop itemAgua = null;
    private Prop itemCarne = null;
    private Prop itemSalgadinho = null;

    private bool bebendo = false;
    private bool comendoCarne = false;
    private bool comendoSalgadinho = false;

    // Props
    private readonly Model garrafaModel = new Model("prop_ld_flow_bottle");
    private readonly Model carneModel   = new Model("prop_cs_steak");
    private readonly Model salgadinhoModel = new Model("prop_cs_crisps_01");

    public ComidaEBebida()
    {
        Tick += OnTick;
        Interval = 50;
    }

    private void OnTick(object sender, EventArgs e)
    {
        Ped player = Game.Player.Character;
        if (!player.Exists() || player.IsDead)
            return;

        // ================== ÁGUA ==================
        if (EstaBebendo(player))
        {
            if (!bebendo)
            {
                bebendo = true;
                CriarItem(ref itemAgua, player, garrafaModel,
                    0.105f, -0.03f, 0.03f,
                    -105f, -25f, -60f);
            }
        }
        else if (bebendo)
        {
            bebendo = false;
            RemoverItem(ref itemAgua);
        }

        // ================== CARNE ==================
        if (EstaComendoCarne(player))
        {
            if (!comendoCarne)
            {
                comendoCarne = true;
                CriarItem(ref itemCarne, player, carneModel,
                    0.16f, 0.025f, 0.05f,
                    -10f, -60f, -30f);
            }
        }
        else if (comendoCarne)
        {
            comendoCarne = false;
            RemoverItem(ref itemCarne);
        }

        // ================== SALGADINHO / LATA ==================
        if (EstaComendoSalgadinho(player))
        {
            if (!comendoSalgadinho)
            {
                comendoSalgadinho = true;
                CriarItem(ref itemSalgadinho, player, salgadinhoModel,
                     0.15f, -0.08f, 0.09f,    // posição ajustada
					45f, 110f, 60f);
            }
        }
        else if (comendoSalgadinho)
        {
            comendoSalgadinho = false;
            RemoverItem(ref itemSalgadinho);
        }
    }

    // ================== DETECÇÃO DE ANIMAÇÕES ==================
    private bool EstaBebendo(Ped ped)
    {
        return Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM,
            ped.Handle, "mp_player_intdrink", "loop_bottle", 3);
    }

    private bool EstaComendoCarne(Ped ped)
    {
        return Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM,
            ped.Handle, "mp_player_inteat@burger", "mp_player_int_eat_burger", 3);
    }

    private bool EstaComendoSalgadinho(Ped ped)
    {
        return Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM,
            ped.Handle, "mp_player_inteat@pnq", "loop", 1);
    }

    // ================== CRIAR ITEM ==================
    private void CriarItem(ref Prop prop, Ped player, Model model,
        float x, float y, float z, float rx, float ry, float rz)
    {
        if (prop != null && prop.Exists())
            RemoverItem(ref prop);

        model.Request();
        if (!model.IsLoaded)
            return;

        prop = World.CreateProp(model, player.Position, true, false);
        Function.Call(Hash.SET_ENTITY_COLLISION, prop.Handle, false, false);

        int bone = Function.Call<int>(Hash.GET_PED_BONE_INDEX, player.Handle, 18905); // SKEL_L_Hand

        Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY,
            prop.Handle, player.Handle, bone,
            x, y, z,
            rx, ry, rz,
            false, false, false, false, 2, true);
    }

    // ================== REMOVER ITEM ==================
    private void RemoverItem(ref Prop prop)
    {
        if (prop != null && prop.Exists())
            prop.Delete();
        prop = null;
    }
}
