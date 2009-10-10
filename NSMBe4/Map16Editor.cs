﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NSMBe4
{
    public partial class Map16Editor : UserControl
    {
        NSMBTileset t;
        NSMBTileset.Map16Tile selTile = null;
        NSMBTileset.Map16Quarter selQuarter = null;
        int selTileNum = -1;

        public delegate void mustRepaintObjectsD();
        public event mustRepaintObjectsD mustRepaintObjects;

        public Map16Editor()
        {
            InitializeComponent();
        }

        public void load(NSMBTileset t)
        {
            this.t = t;
            map16Picker1.SetTileset(t);
            tilePicker1.SetTileset(t);
        }

        private void selectTile(int tile)
        {
            this.selTile = t.Map16[tile];
            this.selTileNum = tile;
            tileBehavior.setArray(t.TileBehaviors[tile]);
            selectQuarter(selTile.topLeft);
        }

        private void selectQuarter(NSMBTileset.Map16Quarter selQuarter)
        {
            this.selQuarter = selQuarter;
            updateQuarterInfo();
        }

        private void updateQuarterInfo()
        {
            int val = selQuarter.TileNum - t.Map16QuarterOffset;
            if (val < -1)
                val = -1;
            tileNumber.Value = val;
            tileByte.Value = selQuarter.TileByte;
            controlByte.Value = selQuarter.ControlByte;
            tilePicker1.selectTile(selQuarter.TileNum - t.Map16QuarterOffset + ((selQuarter.ControlByte & 16) != 0? 448:0));
        }

        private void map16Picker1_TileSelected(int tile)
        {
            selectTile(tile);
        }

        private NSMBTileset.Map16Quarter createQuarter(int tile, bool secondPalette)
        {
            NSMBTileset.Map16Quarter q = new NSMBTileset.Map16Quarter();
            if (secondPalette)
                q.ControlByte = 16;

            q.TileNum = tile + t.Map16QuarterOffset;

            return q;

        }
        private void tilePicker1_TileSelected(int tile)
        {
            bool secondPalette = false;
            if (tile >= 448)
            {
                secondPalette = true;
                tile -= 448;
            }

            if (Control.ModifierKeys == Keys.Control)
            {
                if (selTile == null)
                    return;

                selTile.topLeft = createQuarter(tile, secondPalette);
                selTile.topRight = createQuarter(tile + 1, secondPalette);
                selTile.bottomLeft = createQuarter(tile + 32, secondPalette);
                selTile.bottomRight = createQuarter(tile + 33, secondPalette);

                selectTile(selTileNum);
                repaint();
            }
            else
            {
                if (selQuarter == null)
                    return;

                if (secondPalette != ((selQuarter.ControlByte & 16) != 0))
                    selQuarter.ControlByte ^= 16; //change byte

                selQuarter.TileNum = tile + t.Map16QuarterOffset;
                repaint();
            }
        }

        private void editQ1_Click(object sender, EventArgs e)
        {
            if (selTile == null)
                return;
            selectQuarter(selTile.topLeft);
        }

        private void editQ3_Click(object sender, EventArgs e)
        {

            if (selTile == null)
                return;
            selectQuarter(selTile.topRight);
        }

        private void editQ2_Click(object sender, EventArgs e)
        {

            if (selTile == null)
                return;
            selectQuarter(selTile.bottomLeft);
        }

        private void editQ4_Click(object sender, EventArgs e)
        {

            if (selTile == null)
                return;
            selectQuarter(selTile.bottomRight);
        }

        private void tileNumber_ValueChanged(object sender, EventArgs e)
        {
            if (selQuarter == null)
                return;

            selQuarter.TileNum = (int)tileNumber.Value + t.Map16QuarterOffset;
            repaint();
        }

        private void repaint()
        {
            t.RenderMap16Tile(selTileNum);
            if (mustRepaintObjects != null)
                mustRepaintObjects();
            updateQuarterInfo();
        }

        private void controlByte_ValueChanged(object sender, EventArgs e)
        {
            if (selQuarter == null)
                return;

            selQuarter.ControlByte = (byte)controlByte.Value;
            repaint();
        }

        private void tileByte_ValueChanged(object sender, EventArgs e)
        {
            if (selQuarter == null)
                return;

            selQuarter.TileByte = (byte)tileByte.Value;
            repaint();
        }

        public void redrawThings()
        {
            map16Picker1.SetTileset(t);
        }
    }
}
