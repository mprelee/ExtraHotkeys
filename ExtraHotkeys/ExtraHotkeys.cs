using Assets.Code;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using static HarmonyLib.Code;

namespace ExtraHotkeys
{
    public class ExtraHotkeys
    {
        
        static readonly Dictionary<char, System.Action> extraHotkeysDict = new Dictionary<char, System.Action>()
        {
            // Top Row
            { 'r', TogglePOI<Sub_AncientRuins> },
            { 't', ToggleAgentCreation },
            { 'y', TogglePOI<Sub_WitchCoven> },
            { 'u', TogglePOI<Sub_Sewers> },
            // { 'i', ToggleInternational}, // TODO
            { 'o', FocusChosenOne },
            { 'p', ToggleModifierViewer<Pr_DeepOneCult> },

            // Home Row
            { 'f', TogglePOI<Sub_Farms> },
            { 'g', ToggleGodPowers } ,
            { 'h', ToggleHolyOrders },
            // { 'j', <unused>},
            { 'k', TogglePOI<Sub_Docks> },
            { 'l', TogglePOI<Sub_Library> },

            // Bottom row
            { 'c', () => ToggleGodModifiers(1) },
            { 'v', () => ToggleGodModifiers(2) },
            { 'b', ToggleModifierViewer<Pr_Bandity> },
            { 'n', ToggleModifierViewer<Pr_ArcaneSecret> },
            { 'm', ToggleModifierViewer<Pr_GeomanticLocus> }

        };

        public static bool IsPressed(char key)
        {

            return Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), key.ToString(), true));
        }
	    
        public static void Hotkeys()
        {
            World world = GraphicalMap.world;   
            if(world.ui.uiTopRight.cheatField.isFocused || world.ui.uiLeftPrimary.notes.isFocused || world.ui.uiTopRight.mapGenInputField.isFocused || world.ui.uiScrollables.scrollable_threats.filterField.isFocused || world.ui.uiScrollables.scrollable_locs.filter.isFocused)
            {
                return;
            }


            if (GraphicalMap.world.ui.state == UIMaster.uiState.WORLD)
            {
                foreach(var hotkey in extraHotkeysDict)
                {
                    if(IsPressed(hotkey.Key))
                    {
                        hotkey.Value(); 
                    }
                }
            }
        }
        public static void FocusChosenOne()
        {
            GraphicalMap.panTo(GraphicalMap.world.map.awarenessManager.getChosenOne().location);
        }
        public static void ToggleHolyOrders()
        {
            if(!GraphicalMap.world.map.opt_allowHolyOrders)
            {
                return;
            }
            PopupHolyOrder popup = (PopupHolyOrder)UnityEngine.GameObject.FindObjectOfType(typeof(PopupHolyOrder));
            if (popup == null)
            {
                foreach (SocialGroup socialGroup in GraphicalMap.world.map.socialGroups)
                {
                    if(socialGroup != null && socialGroup is HolyOrder)
                    {
                        GraphicalMap.world.prefabStore.popHolyOrder((HolyOrder)socialGroup);
                        break;
                    }
                }
            }
            else
            {
                popup.dismiss();
            }
        }
        public static void ToggleGodPowers()
        {
            PopupGodPowers popup = (PopupGodPowers)UnityEngine.GameObject.FindObjectOfType(typeof(PopupGodPowers));
            if (popup == null)
            {
                GraphicalMap.world.bCastPower();
            }
            else
            {
                popup.dismiss();
            }
        }
        public static void ToggleInternational()
        {
            PopupInternationalView popup = (PopupInternationalView)UnityEngine.GameObject.FindObjectOfType(typeof(PopupInternationalView));
            if (popup == null)
            {
                return;// TODO
                
            }
            else
            {
                //popup.dismiss();
            }
            return;
        }
        public static void ToggleAgentCreation()
        {
            PopupAgentCreation popup = (PopupAgentCreation)UnityEngine.GameObject.FindObjectOfType(typeof(PopupAgentCreation));
            if (popup == null)
            {
                GraphicalMap.world.bCreateAgent();
            }
            else
            {
                popup.dismiss();
            }
        }
        public static void TogglePOI<T>()
        {
            if (GraphicalMap.world.map.masker.mask == MapMaskManager.maskType.POI_VIEWER &&
                    GraphicalMap.world.ui.uiScrollables.scrollable_threats.pinnedSub.GetType() == typeof(T))
            {
                GraphicalMap.world.map.masker.mask = MapMaskManager.maskType.NONE;
                GraphicalMap.checkData();
                GraphicalMap.world.ui.checkData();
            }
            else
            {
                foreach (Location location in GraphicalMap.world.map.locations)
                {
                    Settlement settlement = location.settlement;
                    if (settlement != null)
                    {
                        foreach (Subsettlement sub in settlement.subs)
                        {
                            if (sub.GetType() == typeof(T))
                            {
                                GraphicalMap.world.map.masker.mask = MapMaskManager.maskType.POI_VIEWER;
                                //GraphicalMap.world.ui.uiLeftPrimary.mapMaskName.text = "POI: " + typeof(T).Name;
                                GraphicalMap.world.ui.uiScrollables.scrollable_threats.targetSub = sub;
                                GraphicalMap.world.ui.uiScrollables.scrollable_threats.pinnedSub = sub;
                                GraphicalMap.checkData();
                                GraphicalMap.world.ui.checkData();
                                return;
                            }
                        }
                    }
                }
            }
        }
        public static void ToggleModifierViewer<T>()
        {
            if (GraphicalMap.world.map.masker.mask == MapMaskManager.maskType.MODIFIER_VIEWER &&
                    GraphicalMap.world.ui.uiScrollables.scrollable_threats.pinnedProperty.GetType() == typeof(T))
            {
                GraphicalMap.world.map.masker.mask = MapMaskManager.maskType.NONE;
                GraphicalMap.checkData();
                GraphicalMap.world.ui.checkData();
                ;
            }
            else
            {
                foreach (Location location in GraphicalMap.world.map.locations)
                {
                    if (location != null)
                    {
                        foreach (Property pr in location.properties)
                        {
                            if (pr.GetType() == typeof(T))
                            {
                                GraphicalMap.world.map.masker.mask = MapMaskManager.maskType.MODIFIER_VIEWER;
                                GraphicalMap.world.ui.uiScrollables.scrollable_threats.pinnedProperty = pr;
                                GraphicalMap.world.ui.uiScrollables.scrollable_threats.targetProperty = pr;
                                GraphicalMap.checkData();
                                GraphicalMap.world.ui.checkData();
                                return;
                            }
                        }
                    }
                }
            }
        }
        public static void ToggleGodModifiers(int modifierType=1)
        {
            God god = GraphicalMap.world.chosenGod;
            if (god is God_Cards)
            {

            }
            else if(god is God_Eternity)
            {
                
            }
            else if(god is God_LaughingKing)
            {
                switch(modifierType)
                {
                    case 1: ToggleModifierViewer<Pr_LaughingTome>(); break;
                    case 2: ToggleModifierViewer<Pr_LaughingTomeInert>(); break;
                    default: ToggleModifierViewer<Pr_LaughingTome>(); break;
                }
            }
            else if (god is God_Mammon)
            {
                switch (modifierType)
                {
                    case 1: ToggleModifierViewer<Pr_MammonsInfluence>(); break;
                    case 2: ToggleModifierViewer<Pr_Mammon_SinsEaten>(); break;
                    default: ToggleModifierViewer<Pr_MammonsInfluence>(); break;
                }
            }
            else if (god is God_Ophanim)
            {
                switch (modifierType)
                {
                    case 1: ToggleModifierViewer<Pr_Opha_Faith>(); break;
                    case 2: ToggleModifierViewer<Pr_Opha_Doubt>(); break;
                    default: ToggleModifierViewer<Pr_Opha_Faith>(); break;
                }
            }
            else if (god is God_Snake)
            {

            }
            else if (god is God_Vinerva)
            {
                switch (modifierType)
                {
                    case 1: TogglePOI<Sub_Vinerva_HeartOfForest>(); break;
                    case 2: ToggleModifierViewer<Pr_Vinverva_Gold>(); break;
                    default: TogglePOI<Sub_Vinerva_HeartOfForest>(); break;
                }
            }
            else
            { 
             
            }
        }
    }
}

