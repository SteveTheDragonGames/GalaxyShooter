using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    private float _powerUpTimer = 5.0f;

    //Borders of screen. Player cannot cross them.
    private float _maxHeight, _minHeight, _minWidth, _maxWidth;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    
    public int ammoCount;
    private Sprite _laserHealthBar;

    [SerializeField]
    private AudioClip _ammoBuzzer;

    private GameObject _thruster;
    //[HideInInspector]
    public bool _isMoving;
    [SerializeField]
    private AudioSource _thrusterSound;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private AudioClip _laser_Clip;
    [SerializeField]
    private AudioClip _thrust_Clip;
    [SerializeField]
    private AudioClip _explosion_Clip;
    [SerializeField]
    private GameObject _explosion_anim;

    private GameObject _rightThruster;
    private GameObject _leftThruster;

    public float _thrusterPower;
    private float _thrusterUseSpeed;
    private float _thrusterRefillSpeed;
    private bool _canMove;


    
    public int _playerLives = 3;

    private SpawnManager _spawnManager;
    private int _weapon = 0; // 0 regular lasers, 1 missiles.

    //Powerups
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private bool _isTripleShotActive;

    [SerializeField]
    private GameObject _Missile;
    [SerializeField]
    public int missileCount = 0;



    [SerializeField]
    private float _speed = 5f;
    private float _speedBoost = 2f;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shield;
    private int _shieldPower;

    private GameObject _healthUp;

    [HideInInspector]
    public int powerUpID;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;
    private GameManager _gameManager;

    AudioSource _audioSource;
    public ShakeCamera shakeCamera;

    void Start()
    {   
        shakeCamera = GameObject.Find("CameraShaker").GetComponent<ShakeCamera>();
        if(!shakeCamera)
        {
            Debug.LogError("CameraShaker is null");
        }

        _audioSource = GetComponent<AudioSource>();
        if(!_audioSource){
            Debug.LogError("AudioSource in the Player is null");
        }
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(!_spawnManager)
        {
            Debug.LogError("Spawn Manager in the Player is null");
        }
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (!_gameManager){
            Debug.LogError("Game Manager in the Player is null");
        }

        //Thruster sprites and turning them off.
        _rightThruster = GameObject.Find("Right_Thruster");
        _leftThruster = GameObject.Find("Left_Thruster");
        _thruster = GameObject.Find("Thruster");

        _thruster.gameObject.SetActive(false);
        _rightThruster.gameObject.SetActive(false);
        _leftThruster.gameObject.SetActive(false);
        
        //Initiate Shields
        _shield.SetActive(false);
        _shieldPower = 0;       

        if(_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is NULL!");
        }
        
        //Initiate Player healthup animation
        _healthUp = GameObject.Find("HealthUpAnim");
        _healthUp.SetActive(false);

        //Initializing the players position
        transform.position = new Vector3(0,0,0);
        _maxHeight =  0f;
        _minHeight = -3.8f;
        _maxWidth  =  9f;
        _minWidth  = -_maxWidth;
        _speedBoost = 1f;

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("UIManager is null");
        }

        ammoCount = 15;
        missileCount = 0;
        _thrusterPower = 100f;
        _thrusterUseSpeed = 20f;
        _thrusterRefillSpeed = 35f;
        _canMove = true;

        _uiManager.UpdateAmmoCount();
        _uiManager.UpdateMissileCount();

        
    }

    void Update()
    {
        CheckKeyPress();
       CalculateMovement();
       CheckBorders();
       TurnThrustersOn();
       CheckBooster();
       Shields();
       ThrusterRefill();
    }
    void CheckKeyPress()
    {
        //check the movement keys
        if(Input.GetKey("up") || Input.GetKey("down") ||
         Input.GetKey("left") || Input.GetKey("right") || 
         Input.GetKey("w") || Input.GetKey("a") || 
         Input.GetKey("s") || Input.GetKey("d"))
        {
            _isMoving = true;   
        }
        else
        {
            _isMoving = false;
            StartCoroutine(ThrusterRefill());
        }

        //Check the fire button
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
       {
           switch (_weapon) //0 laser, 1 missiles
           {
            case 0:
                FireLaser();
                break;
            case 1:
                FireMissile();
                break;
           }
       }


    }


    void TurnThrustersOn()
    {
        
        if(_isMoving && _canMove)
        {   _thruster.gameObject.SetActive(true);
            _thrusterSound.Play();

            _thrusterPower -= _thrusterUseSpeed * Time.deltaTime;
            if(_thrusterPower < 0)
            {
                _thrusterPower = 0;
                _canMove = false;
            }
            _uiManager.UpdateThrusterCount();
        }else
        {
            _thruster.gameObject.SetActive(false);
            _thrusterSound.Stop();
        }
    }

    IEnumerator ThrusterRefill()
    {
        
        yield return new WaitForSeconds(1f);
        if(!_isMoving)
        {
            _thrusterPower += _thrusterRefillSpeed * Time.deltaTime;
            if (_thrusterPower >= 100)
            {
                _thrusterPower = 100;
            }
            _uiManager.UpdateThrusterCount();
            _canMove=true;
            
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            _canMove=true;
        }
    }

    void CalculateMovement()
    {
        if(_canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(horizontalInput, verticalInput,0)
            *_speed * _speedBoost * Time.deltaTime);
        }
    }

    void CheckBooster()
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            _speedBoost = 3f;//setting speedboost to 3 so it's noticable. Normal would be 1.3 or 1.5
        }
        else
        {
            _speedBoost = 1f;
        }
    }

    void CheckBorders()
    {
        if (transform.position.y > _maxHeight)
        {
            transform.position = new Vector3(transform.position.x, _maxHeight, 0);
        }else if (transform.position.y < _minHeight){
            transform.position = new Vector3(transform.position.x, _minHeight ,0);
        }

        if (transform.position.x > _maxWidth)
        {
            transform.position = new Vector3(_maxWidth, transform.position.y,0);
        }else if(transform.position.x < _minWidth)
        {
            transform.position = new Vector3(_minWidth, transform.position.y,0);
        }
    }

    void FireLaser()
    {
        if(ammoCount > 0)
        {
            ammoCount --;
            _uiManager.UpdateAmmoCount();
            _canFire = Time.time + _fireRate;
            if(_isTripleShotActive)
            {
                ammoCount -= 2;
                _uiManager.UpdateAmmoCount();
                Instantiate(_tripleShot, transform.position, Quaternion.identity);

            }else
            {
                Vector3 offset = new Vector3(0f,.8f,0f);
                Instantiate(_laserPrefab, transform.position+offset, Quaternion.identity);
            }
            _audioSource.clip = _laser_Clip;
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = _ammoBuzzer;
            _audioSource.Play();
        }
        
        if (ammoCount<=0)
        {
            ammoCount = 0;
        }
    }

    void FireMissile()
    {
        if (missileCount > 0)
        {
            missileCount --;
            _uiManager.UpdateMissileCount();

            _canFire = Time.time + _fireRate;
            Vector3 offset = new Vector3(0f,.8f,0f);
            Instantiate(_Missile, transform.position+offset, Quaternion.identity);
        }else{
            _uiManager.UpdateMissileCount();
            _weapon = 0;
        }
    }

    void ShakeCam(float TimeToShake)
    {
        shakeCamera.InitiateShake(TimeToShake);
    }

    public void Damage()
    {
        ShakeCam(.3f);
        
        if (_isShieldActive)
        {
            _shieldPower --;
            return;
        }

            
        _playerLives --;
        
        _uiManager.UpdateLives(_playerLives);

    DamageShip();

        if(_playerLives < 1)
        {
            Instantiate(_explosion_anim, transform.position, Quaternion.identity);
            _gameManager.GameOver();
            _uiManager.GameOverText();
            _spawnManager.OnPlayerDeath();
            PlayExplosionSound();
            gameObject.SetActive(false);
            Destroy(this.gameObject,1);
            
        }
    }

    private void DamageShip()
    {
            switch(_playerLives)
        {
            case 3:
            _leftThruster.gameObject.SetActive(false);
            _rightThruster.gameObject.SetActive(false);
            break;
            case 2:
            _leftThruster.gameObject.SetActive(true);
            _rightThruster.gameObject.SetActive(false);
            break;
            case 1:
            _leftThruster.gameObject.SetActive(true);
            _rightThruster.gameObject.SetActive(true);
            break;
            default:
            break;
        }
    }

    private void Shields()
    {
        if (_shieldPower <=0)
        {
            _shieldPower = 0;
            
        }
        Color tmp = _shield.GetComponent<SpriteRenderer>().color;
            switch(_shieldPower)
            {
 
                case 3:

                tmp.a = 1f;
                _shield.GetComponent<SpriteRenderer>().color = tmp;
                _shield.GetComponent<SpriteRenderer>().color = Color.white;
                _isShieldActive = true;
                break;
                case 2:

                tmp.a = 0.5f;
                _shield.GetComponent<SpriteRenderer>().color = tmp;
                _shield.GetComponent<SpriteRenderer>().color = Color.magenta;
                _isShieldActive = true;
                break;
                case 1:

                tmp.a = .1f;
                _shield.GetComponent<SpriteRenderer>().color = tmp;
                _shield.GetComponent<SpriteRenderer>().color = Color.red;
                _isShieldActive = true;
                break;
                case 0:
                _isShieldActive = false;
                _shield.SetActive(false);
                break;

            }
            return;
 
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        StartCoroutine(FinishPowerUp());
    }

    public void ActivateSpeedBoost()
    {
        _speedBoost = 2f;
        StartCoroutine(FinishPowerUp());
    }

    public void ActivateShields()
    {
        _shieldPower = 3;
        _isShieldActive = true;
        _shield.SetActive(true);
    }

    public void AddAmmo()
    {
        ammoCount = 15;
        _uiManager.UpdateAmmoCount();
    }

    public void AddHealth()
    {
        _healthUp.SetActive(true);
        _healthUp.GetComponent<Animator>().Play("HealthUp");
        Invoke ("TurnOffHealthUp", .5f);
        _playerLives ++;
        if(_playerLives >=3)
        {
            _playerLives = 3;            
        }
        _uiManager.UpdateLives(_playerLives);
        DamageShip();
    }
    public void ActivateMissiles()
    {
        _weapon = 1;
        missileCount = 5;
        _uiManager.UpdateMissileCount();
    }

    private void TurnOffHealthUp()
    {
        _healthUp.SetActive(false);
    }
    IEnumerator FinishPowerUp()
    {
        yield return new WaitForSeconds(_powerUpTimer);
        switch(powerUpID)
        {
        case 0:
            _isTripleShotActive = false;
            break;
            
        case 1:
            _speedBoost = 1f;
            break;  
        }
    }

    public void AddScore(int points)
    {
        _score+=points; 
        _uiManager.UpdateScore(_score);       
    }

    public void PlayExplosionSound()
    {
        _audioSource.clip = _explosion_Clip;
        _audioSource.Play();
    }


}


 