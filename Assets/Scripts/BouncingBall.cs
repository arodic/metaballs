using UnityEngine;
using System.Collections;

public class BouncingBall : MetaBall {
    public float speed;

    private Container container;
    private Vector3 direction;

    public override void Start() {
        base.Start();
        direction = Random.onUnitSphere;
        container = GetComponentInParent<Container>();
    }

    public void Update() {
        updatePosition(Time.deltaTime);
    }

    public void updatePosition(float dt) {
        float posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z;
        Vector3 containerPosition = container.transform.position;
        Vector3 containerScale = container.transform.localScale;

        if(posX + radius + container.safeZone > containerPosition.x + containerScale.x / 2) {
            posX -= 0.01f;
            direction = Vector3.Reflect(direction, Vector3.left);
        } else if(posX - radius - container.safeZone < containerPosition.x - containerScale.x / 2) {
            posX += 0.01f;
            direction = Vector3.Reflect(direction, Vector3.right);
        }

        if(posY + radius + container.safeZone > containerPosition.y + containerScale.y / 2) {
            posY -= 0.01f;
            direction = Vector3.Reflect(direction, Vector3.down);
        } else if(posY - radius - container.safeZone < containerPosition.y - containerScale.y / 2) {
            posY += 0.01f;
            direction = Vector3.Reflect(direction, Vector3.up);
        }

        if(posZ + radius + container.safeZone > containerPosition.z + containerScale.z / 2) {
            posZ -= 0.01f;
            direction = Vector3.Reflect(direction, Vector3.back);
        } else if(posZ - radius - container.safeZone < containerPosition.z - containerScale.z / 2) {
            posZ += 0.01f;
            direction = Vector3.Reflect(direction, Vector3.forward);
        }

        transform.position = new Vector3(posX, posY, posZ) + direction * speed * dt;
    }
}
