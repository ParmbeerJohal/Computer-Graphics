/*
 * Parm Johal
 * V00787710
 * CSC 305
 * Assignment #1
 * Ray-sphere intersection
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RayTracer : MonoBehaviour
{

    //initialize variables
    public Texture2D texture_on_cube;
    Texture2D RayTracingResult;
    Vector3 LightDirection = new Vector3(1, 2, 3);
    Color LightColor = Color.yellow;
    Vector3 SphereCenter = new Vector3(0, 0, 5);
    float SphereRadius = 3;

    void Start()
    {

        Camera this_camera = gameObject.GetComponent<Camera>();
        Debug.Assert(this_camera);

        int pixel_width = this_camera.pixelWidth;
        int pixel_height = this_camera.pixelHeight;

        RayTracingResult = new Texture2D(pixel_width, pixel_height);



        #region Generate a black and white checker pattern

        for (int i = 0; i < pixel_width; ++i)
            for (int j = 0; j < pixel_height; ++j)
            {
                int iblock = i / 50;
                int jblock = j / 50;
                RayTracingResult.SetPixel(i, j,
                    (iblock + jblock) % 2 == 0 ? Color.black : Color.white);
            }
        #endregion

        //Initialize screenspace/Rays/colors/shading variables
        Vector3 RayOrigin = Vector3.zero;
        Vector3 VPCenter = Vector3.forward;
        float Viewportwidth = 3;
        float Viewportheight = Viewportwidth / pixel_width * pixel_height;

        float VPWidthHalf = Viewportwidth / 2;
        float VPHeightHalf = Viewportheight / 2;

        float PixelWidthHalf = pixel_width / 2;
        float PixelHeightHalf = pixel_height / 2;

        Color BackgroundColor = Color.grey;
        Color PixelColor;
        Color AmbientColor = new Color(0.1f, 0.1f, 0); //rgba values

        float diffuseStrength = 0.0005f; 
        float specularStrength = 0.0004f;
        float power = 4f;

        Vector3 RayDirection = VPCenter;

        // The following nested for loop iterates through each pixel and
        // determines whether or not the ray going through the current pixel
        // intersects with the sphere.
        for (int i = 0; i < pixel_width; ++i)
        {
            for (int j = 0; j < pixel_height; ++j)
            {
                RayDirection.x = (i - PixelWidthHalf) / PixelWidthHalf * VPWidthHalf; //current pixel - half of pixel width / half of pixel width * view port width
                RayDirection.y = (j - PixelHeightHalf) / PixelHeightHalf * VPHeightHalf;

                RayTracingResult.SetPixel(i, j, Color.grey);
                RayDirection.Normalize();

                PixelColor = BackgroundColor;

                //initialize vectors
                Vector3 position = Vector3.zero;
                Vector3 Intersect_Normal;

                Vector3 CO = SphereCenter - RayOrigin;
                float OG = Vector3.Dot(CO, RayDirection);
                

                float Discriminent = SphereRadius * SphereRadius - (Vector3.Dot(CO, CO) - OG * OG);
                
                // If ray intersects with the sphere
                if (Discriminent > 0)
                {
                    float t = OG - Mathf.Sqrt(Discriminent);
                    position = RayOrigin + t * RayDirection;
                    Intersect_Normal = position - SphereCenter;



                    //Ambient
                    PixelColor = AmbientColor;

                    //Diffuse
                    float diffuse = Vector3.Dot(Intersect_Normal, LightDirection) * diffuseStrength;  
                    PixelColor += LightColor * diffuse;

                    //Specular
                    Vector3 view = RayDirection * (-1);
                    Vector3 half = view + LightDirection;
                    float blinn = Vector3.Dot(half, Intersect_Normal);
                    float specular = Mathf.Pow(blinn, power) * specularStrength;
                    PixelColor += LightColor * specular;

                    RayTracingResult.SetPixel(i, j, PixelColor);
                }
            }
        }

        RayTracingResult.Apply();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Show the generated ray tracing image on screen
        Graphics.Blit(RayTracingResult, destination);
    }
}