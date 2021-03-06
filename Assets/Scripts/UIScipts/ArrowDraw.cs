﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArrowDraw : MonoBehaviour
{
    public struct DrawnArrow
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
        public GameObject arrowGameObject;
        public GameObject arrowHead;
    }

    public struct DrawnHighlightSquare
    {
        public int x1;
        public int y1;
        public GameObject highlightGameObject;
    }

    private List<DrawnArrow> drawnArrows = new List<DrawnArrow>();
    private List<DrawnHighlightSquare> drawnSquares = new List<DrawnHighlightSquare>();

    private int currentX;
    private int currentY;
    private int startX;
    private int startY;

    public int arrowHeight;
    public float arrowWidth;
    private bool isDrawingArrow;

    public GameObject hightlightSquare;
    public GameObject lineDrawer;
    public GameObject arrowHead;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

        currentX = Mathf.RoundToInt(mousePos.x);
        currentY = Mathf.RoundToInt(mousePos.z);

        if (Input.GetMouseButtonDown(1))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            EndDrawing();
        }
    }

    void StartDrawing()
    {
        startX = Mathf.RoundToInt(currentX);
        startY = Mathf.RoundToInt(currentY);
    }

    void EndDrawing()
    {
        if (startX <= 7 && startX >= 0 && startY <= 7 && startY >= 0 && currentX <= 7 && currentX >= 0 && currentY <= 7 && currentY >= 0)
        {
            if (startX == currentX && startY == currentY)
            {
                HighlightSquare(startX, startY);
            }
            else
            {
                DrawArrow(startX, startY, currentX, currentY);
            }
        }
    }

    public void HighlightSquare(int xCoord, int yCoord)
    {
        bool z = false;
        for (int i = 0; i < drawnSquares.Count; i++)
        {
            if (xCoord == drawnSquares[i].x1 && yCoord == drawnSquares[i].y1)
            {
                Destroy(drawnSquares[i].highlightGameObject);
                drawnSquares.RemoveAt(i);
                z = true;
            }
        }
        if (z != true)
        {
            DrawnHighlightSquare temp;
            temp.x1 = xCoord;
            temp.y1 = yCoord;
            temp.highlightGameObject = Instantiate(hightlightSquare, new Vector3(xCoord, 0.6f, yCoord), Quaternion.identity);
            temp.highlightGameObject.transform.SetParent(this.transform);
            drawnSquares.Add(temp);
        }
    }

    void DrawArrow(int x1, int y1, int x2, int y2)
    {
        bool z = false;
        for (int i = 0; i < drawnArrows.Count; i++)
        {
            if (x1 == drawnArrows[i].x1 && y1 == drawnArrows[i].y1 && x2 == drawnArrows[i].x2 && y2 == drawnArrows[i].y2)
            {
                Destroy(drawnArrows[i].arrowGameObject);
                drawnArrows.RemoveAt(i);
                z = true;
            }          
        }
        if (z != true)
        {
            DrawnArrow temp;
            temp.x1 = x1;
            temp.x2 = x2;
            temp.y1 = y1;
            temp.y2 = y2;

            temp.arrowGameObject = Instantiate(lineDrawer, new Vector3(this.transform.position.x, arrowHeight, this.transform.position.z), Quaternion.identity);
            temp.arrowGameObject.transform.SetParent(this.transform);
            LineRenderer Lr = temp.arrowGameObject.GetComponent<LineRenderer>();

            float bearing = 0;
            float dx = (x2 - x1);
            float dy = (y2 - y1);

            (float x, float y) diffVector = ((x2 - x1), (y2 - y1));
            float absDiffVector = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            float arrowOffset = 1 - 0.486f / absDiffVector;
            float lineOffset = 0.3f / absDiffVector;

            (float x, float y) startVector = (x1 + (lineOffset * diffVector.x), y1 + (lineOffset * diffVector.y));
            (float x, float y) endVector = (x1 + (arrowOffset * diffVector.x), y1 + (arrowOffset * diffVector.y));

            if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 2)
            {
                bearing = 90;
                if (dx < 0) bearing = 270;
                temp.arrowHead = Instantiate(arrowHead, new Vector3(x2, arrowHeight, y2), Quaternion.Euler(90, bearing, 0));
                temp.arrowHead.transform.SetParent(temp.arrowGameObject.transform);

                startVector = (x1, y1 + (lineOffset * diffVector.y));
                endVector = (x1, y1 + dy);

                drawnArrows.Add(temp);
                Lr.startWidth = arrowWidth + 0.088f;
                Lr.endWidth = arrowWidth + 0.088f;
                Lr.positionCount = 3;
                Lr.numCapVertices = 1;
                Lr.SetPosition(0, new Vector3(startVector.x, arrowHeight, y1 + (0.2f * dy)));
                Lr.SetPosition(1, new Vector3(endVector.x, arrowHeight, endVector.y));
                Lr.SetPosition(2, new Vector3(x2 - (0.59f * dx), arrowHeight, y2));
                return;
            }
            else if (Mathf.Abs(dx) == 2 && Mathf.Abs(dy) == 1)
            {
                bearing = 0;
                if (dy < 0) bearing = 180;
                temp.arrowHead = Instantiate(arrowHead, new Vector3(x2, arrowHeight, y2), Quaternion.Euler(90, bearing, 0));
                temp.arrowHead.transform.SetParent(temp.arrowGameObject.transform);

                startVector = (x1 + (lineOffset * diffVector.x), y1);
                endVector = (x1 + dx, y1);
                
                drawnArrows.Add(temp);
                Lr.startWidth = arrowWidth + 0.088f;
                Lr.endWidth = arrowWidth + 0.088f;
                Lr.positionCount = 3;
                Lr.numCapVertices = 1;
                Lr.SetPosition(0, new Vector3(x1 + (0.2f * dx), arrowHeight, startVector.y));
                Lr.SetPosition(1, new Vector3(endVector.x, arrowHeight, endVector.y));
                Lr.SetPosition(2, new Vector3(x2, arrowHeight, y2 - (0.59f * dy)));
                return;
            }
            else if (dx > 0 && dy > 0) // Upper Right
            {
                dx = Mathf.Abs(dx);
                dy = Mathf.Abs(dy);
                bearing = Mathf.Atan(dy / dx); //Upper Right
                bearing = 90 - bearing * Mathf.Rad2Deg;
            }
            else if (dx > 0 && dy < 0) // Lower Right
            {
                dx = Mathf.Abs(dx);
                dy = Mathf.Abs(dy);
                bearing = Mathf.Atan(dy / dx); // Lower Right
                bearing = bearing * Mathf.Rad2Deg + 90;
            }
            else if (dx < 0 && dy < 0) // Lower Left
            {
                dx = Mathf.Abs(dx);
                dy = Mathf.Abs(dy);
                bearing = Mathf.Atan(dy / dx); // Lower Left
                bearing = 270 - bearing * Mathf.Rad2Deg;
            }
            else if (dx < 0 && dy > 0) // Upper Left
            {
                dx = Mathf.Abs(dx);
                dy = Mathf.Abs(dy);
                bearing = Mathf.Atan(dy / dx); // Upper Left
                bearing = bearing * Mathf.Rad2Deg + 270;
            }
            else if (dx == 0)
            {
                if (dy > 0)
                {
                    bearing = 0;
                }
                else if (dy < 0)
                {
                    bearing = 180;
                }
            }
            else if (dy == 0)
            {
                if (dx > 0)
                {
                    bearing = 90;
                }
                else if (dx < 0)
                {
                    bearing = 270;
                }
            }
            temp.arrowHead = Instantiate(arrowHead, new Vector3(x2, arrowHeight, y2), Quaternion.Euler(90, bearing, 0));
            temp.arrowHead.transform.SetParent(temp.arrowGameObject.transform);

            drawnArrows.Add(temp);
            Lr.startWidth = arrowWidth;
            Lr.endWidth = arrowWidth;
            Lr.SetPosition(0, new Vector3(startVector.x, arrowHeight, startVector.y));
            Lr.SetPosition(1, new Vector3(endVector.x, arrowHeight, endVector.y));           
        }
    }
}
