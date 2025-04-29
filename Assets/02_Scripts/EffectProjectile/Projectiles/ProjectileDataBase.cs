using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect&Projectile/ProjectileDataBase")]
public class ProjectileDataBase : ScriptableObject
{
    public List<ProjectileData> projectileDatas;
}
