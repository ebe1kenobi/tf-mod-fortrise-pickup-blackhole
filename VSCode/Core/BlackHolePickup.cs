using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace TFModFortRisePickupBlackHole
{
  [CustomPickup("BlackHolePickup", "0.0")]
  public class BlackHolePickup : Pickup
  {
    private const float ATTRACTION_RADIUS = 60f;
    private const float ATTRACTION_FORCE = 2.5f;
    private const float TELEPORT_RADIUS = 5f;
    private Counter lifeCounter;
    private Counter portal;
    private Vector2 position;
    private Sprite<int> sprite;
    //private Sprite<int> output;
    bool done = false;
    private Vector2 teleportPosition;
    private bool outputFound = false;

    public BlackHolePickup(Vector2 position, Vector2 targetPosition, Pickups pickupType)
        : base(position, targetPosition)
    {
      this.position = position;
      base.Collider = new WrapHitbox(ATTRACTION_RADIUS * 2, ATTRACTION_RADIUS * 2, -ATTRACTION_RADIUS, -ATTRACTION_RADIUS);
      this.sprite = TFGame.SpriteData.GetSpriteInt("SpawnPortal");
      this.sprite.Play(0, false);
      this.sprite.CenterOrigin();
      base.Add(this.sprite);
      this.Tag(GameTags.PlayerCollectible);
      this.lifeCounter = new Counter();
      this.lifeCounter.Set(300);

    }
    public override void Update()
    {
      base.Update();


      if (!outputFound && !TFModFortRisePickupBlackHoleModule.Settings.random)
      {
        outputFound = true;
        Vector2 offset = Vector2.Zero;
        float width = 16f;
        float height = 24f;
        int attempts = 0;
        do
        {
          teleportPosition = FindSafePosition(width, height);
          // Ajuster la position en fonction de l'offset du collider
          Vector2 testPos = teleportPosition - offset;
          if (!IsSolidAt(testPos, width, height))
          {
            break;
          }
          attempts++;
        } while (attempts < 50);

        // Create portal effects and teleport
        Sprite<int> destSprite = TFGame.SpriteData.GetSpriteInt("SpawnPortal");
        Entity destPortal = new Entity(teleportPosition);
        destPortal.Add(destSprite);
        destSprite.Play(0, false);
        Level.Add(destPortal);
      }

      if (this.Collidable)
      {
        this.sprite.Scale.X = 2f + 0.05f * this.sine.ValueOverTwo;
        this.sprite.Scale.Y = 2f + 0.05f * this.sine.Value;
      }
      this.sprite.Position = base.DrawOffset;
      // Attirer les joueurs et les flèches à proximité
      foreach (Entity entity in this.Level[GameTags.Player])
      {
        Player player = entity as Player;
        if (player != null)
        {
          AttractionEffect(player);
        }
      }

      // Attirer les flèches
      foreach (Entity entity in this.Level[GameTags.Arrow])
      {
        Arrow arrow = entity as Arrow;
        if (arrow != null && arrow.State != Arrow.ArrowStates.Stuck)
        {
          AttractionEffect(arrow);
        }
      }
    }

    private void AttractionEffect(Entity entity)
    {
      Vector2 direction = this.Position - entity.Position;
      float distance = direction.Length();

      if (distance < ATTRACTION_RADIUS)
      {
        // Calculer la force d'attraction
        float force = (1 - (distance / ATTRACTION_RADIUS)) * ATTRACTION_FORCE;
        direction.Normalize();

        // Appliquer la force
        if (entity is Player)
        {
          Player player = entity as Player;
          player.Speed += direction * force * Engine.TimeMult;
        }
        else if (entity is Arrow)
        {
          Arrow arrow = entity as Arrow;
          arrow.Speed += direction * force * Engine.TimeMult;
        }

        // Téléporter si assez proche
        if (distance < TELEPORT_RADIUS)
        {
          TeleportEntity(entity);
        }
      }
    }

    private Vector2 FindSafePosition(float width, float height)
    {
      const int MARGIN = 32;
      Vector2 pos;
      int attempts = 0;
      bool found = false;

      do
      {
        // Générer une position aléatoire avec une meilleure distribution
        pos = new Vector2(
            Calc.Random.Range(MARGIN, 320 - MARGIN),
            Calc.Random.Range(MARGIN, 240 - MARGIN)
        );

        // Vérifier si la position est sûre pour un joueur
        if (!IsSolidAt(pos, 16, 24))
        {
          found = true;
          break;
        }

        attempts++;
      } while (attempts < 50);

      // Si aucune position sûre n'est trouvée après 50 tentatives,
      // essayer de trouver une position plus en hauteur
      if (!found)
      {
        for (float y = MARGIN; y < 240 - MARGIN; y += 8)
        {
          for (float x = MARGIN; x < 320 - MARGIN; x += 8)
          {
            pos = new Vector2(x, y);
            if (!IsSolidAt(pos, width, height))
            {
              return pos;
            }
          }
        }
      }

      return pos;
    }

    private bool IsSolidAt(Vector2 position, float width, float height)
    {
      // Créer une hitbox temporaire pour tester les collisions
      WrapHitbox testHitbox = new WrapHitbox(width, height, -width / 2, -height / 2);  // Explicitement centrer la hitbox
      Entity testEntity = new Entity(position);
      testEntity.Collider = testHitbox;

      // Vérifier les collisions avec les solides
      foreach (Entity solid in base.Level[GameTags.Solid])
      {
        if (solid.Collider != null && testEntity.CollideCheck(solid))
        {

          return true;
        }
      }

      // Vérifier si la position est hors des limites du niveau
      if (position.X - width / 2 < 0 || position.X + width / 2 > 320 ||
          position.Y - height / 2 < 0 || position.Y + height / 2 > 240)
      {
        return true;
      }

      return false;
    }

    private void TeleportEntity(Entity entity)
    {
      if (!TFModFortRisePickupBlackHoleModule.Settings.random) {
        // Teleport the entity
        entity.Position = teleportPosition;
        return;
      }
      // Obtenir la taille exacte de l'entité et son offset de collider
      float width = 16f;
      float height = 24f;
      Vector2 offset = Vector2.Zero;

      if (entity.Collider != null)
      {
        width = entity.Collider.Width;
        height = entity.Collider.Height;
        if (entity.Collider is WrapHitbox hitbox)
        {
          offset = new Vector2(hitbox.Left, hitbox.Top);
        }
      }

      Vector2 newPos;
      int attempts = 0;
      bool found = false;

      do
      {
        newPos = FindSafePosition(width, height);
        // Ajuster la position en fonction de l'offset du collider
        Vector2 testPos = newPos - offset;
        if (!IsSolidAt(testPos, width, height))
        {
          found = true;
          break;
        }
        attempts++;
      } while (attempts < 50);

      if (!found)
      {
        newPos = entity.Position;
        return;
      }

      // Create portal effects and teleport
      Sprite<int> destSprite = TFGame.SpriteData.GetSpriteInt("SpawnPortal");
      Entity destPortal = new Entity(newPos);
      destPortal.Add(destSprite);
      destSprite.Play(0, false);
      Level.Add(destPortal);

      // Teleport the entity
      entity.Position = newPos;

      // Remove portals after animation
      destSprite.OnAnimationComplete = (Sprite<int> s) => {
        destPortal.RemoveSelf();
      };
    }

    public override void Render()
    {
      base.DrawGlow();
      this.sprite.DrawOutline(1);
      base.Render();
    }

    public override void TweenUpdate(float t)
    {
      base.TweenUpdate(t);
      this.sprite.Scale = Vector2.One * t;
    }

    private void DebugPrintLevelMap()
    {
      Logger.Info("=== LEVEL MAP ===");

      // Créer une hitbox de 1x1 pour tester chaque pixel
      WrapHitbox testHitbox = new WrapHitbox(1, 1);
      Entity testEntity = new Entity(Vector2.Zero);
      testEntity.Collider = testHitbox;

      for (int y = 0; y < 240; y++)
      {
        string line = "";
        for (int x = 0; x < 320; x++)
        {
          testEntity.Position = new Vector2(x, y);
          bool isSolid = false;

          foreach (Entity solid in base.Level[GameTags.Solid])
          {
            if (solid.Collider != null && testEntity.CollideCheck(solid))
            {
              isSolid = true;
              break;
            }
          }

          line += isSolid ? "1" : "0";
        }
        Logger.Info(line);
      }

      Logger.Info("=== END MAP ===");
    }
  }
}